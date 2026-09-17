namespace MaintenanceDesk.Api.Events;

public sealed class NullEventPublisher : IEventPublisher
{
    public Task PublishAsync(MaintenanceRequestEvent @event, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task ScheduleAsync(MaintenanceRequestEvent @event, DateTimeOffset deliverAt, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
