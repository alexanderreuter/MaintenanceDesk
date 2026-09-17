using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;

namespace MaintenanceDesk.Api.Events;

public sealed class ServiceBusEventPublisher(ServiceBusSender sender) : IEventPublisher
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public Task PublishAsync(MaintenanceRequestEvent @event, CancellationToken cancellationToken = default) =>
        sender.SendMessageAsync(ToMessage(@event), cancellationToken);

    public async Task ScheduleAsync(MaintenanceRequestEvent @event, DateTimeOffset deliverAt, CancellationToken cancellationToken = default) =>
        await sender.ScheduleMessageAsync(ToMessage(@event), deliverAt, cancellationToken);

    private static ServiceBusMessage ToMessage(MaintenanceRequestEvent @event) =>
        new(BinaryData.FromObjectAsJson(@event, Json))
        {
            ContentType = "application/json",
            Subject = @event.Type.ToString(),
        };
}
