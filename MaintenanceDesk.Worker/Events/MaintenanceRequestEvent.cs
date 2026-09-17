namespace MaintenanceDesk.Worker.Events;

public record MaintenanceRequestEvent(Guid RequestId, MaintenanceRequestEventType Type, DateTimeOffset OccurredAt);
