namespace MaintenanceDesk.Api.Events;

public interface IEventPublisher
{
    Task PublishAsync(MaintenanceRequestEvent @event, CancellationToken cancellationToken = default);

    Task ScheduleAsync(MaintenanceRequestEvent @event, DateTimeOffset deliverAt, CancellationToken cancellationToken = default);
}
