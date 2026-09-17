using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using MaintenanceDesk.Worker.Api;
using MaintenanceDesk.Worker.Events;

namespace MaintenanceDesk.Worker;

public class MaintenanceEventWorker(
    ServiceBusProcessor processor,
    MaintenanceRequestsClient api,
    ILogger<MaintenanceEventWorker> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    // Anything else, including a status this worker has never heard of, counts as still open.
    private static readonly string[] SettledStatuses = ["Resolved", "Closed", "Cancelled"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        processor.ProcessMessageAsync += HandleMessageAsync;
        processor.ProcessErrorAsync += HandleErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);
        logger.LogInformation("Listening for maintenance request events on {Queue}.", processor.EntityPath);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Shutting down.
        }
        finally
        {
            await processor.StopProcessingAsync(CancellationToken.None);
        }
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        MaintenanceRequestEvent? @event;

        try
        {
            @event = args.Message.Body.ToObjectFromJson<MaintenanceRequestEvent>(Json);
        }
        catch (JsonException ex)
        {
            // Retrying will not make it parse, so take it off the queue immediately.
            logger.LogError(ex, "Message {MessageId} is not a valid event.", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "InvalidJson", ex.Message, args.CancellationToken);
            return;
        }

        if (@event is null)
        {
            await args.DeadLetterMessageAsync(args.Message, "EmptyBody", "The message body was null.", args.CancellationToken);
            return;
        }

        switch (@event.Type)
        {
            case MaintenanceRequestEventType.Created:
                logger.LogInformation(
                    "Would notify the property manager: maintenance request {RequestId} was reported at {OccurredAt:u}.",
                    @event.RequestId, @event.OccurredAt);
                break;

            case MaintenanceRequestEventType.StatusChanged:
                await LogStatusChangeAsync(@event, args.CancellationToken);
                break;

            case MaintenanceRequestEventType.DeadlineReached:
                await CheckDeadlineAsync(@event, args.CancellationToken);
                break;

            default:
                logger.LogWarning("Ignoring unknown event type {EventType} for request {RequestId}.", @event.Type, @event.RequestId);
                break;
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private async Task LogStatusChangeAsync(MaintenanceRequestEvent @event, CancellationToken cancellationToken)
    {
        var request = await api.GetAsync(@event.RequestId, cancellationToken);

        if (request is null)
        {
            logger.LogWarning("Maintenance request {RequestId} no longer exists.", @event.RequestId);
            return;
        }

        logger.LogInformation(
            "Would notify the resident: maintenance request {RequestId} ({Title}) is now {Status}.",
            request.Id, request.Title, request.Status);
    }

    private async Task CheckDeadlineAsync(MaintenanceRequestEvent @event, CancellationToken cancellationToken)
    {
        var request = await api.GetAsync(@event.RequestId, cancellationToken);

        if (request is null)
        {
            logger.LogWarning("Maintenance request {RequestId} no longer exists.", @event.RequestId);
            return;
        }

        if (SettledStatuses.Contains(request.Status))
        {
            logger.LogInformation(
                "Response deadline reached for maintenance request {RequestId}, already {Status}.",
                request.Id, request.Status);
            return;
        }

        logger.LogWarning(
            "Response deadline breached: maintenance request {RequestId} ({Title}, {Priority}) is still {Status}; deadline was {Deadline:u}.",
            request.Id, request.Title, request.Priority, request.Status, request.ResponseDeadline);
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Service Bus error during {Operation} on {Entity}.", args.ErrorSource, args.EntityPath);
        return Task.CompletedTask;
    }
}
