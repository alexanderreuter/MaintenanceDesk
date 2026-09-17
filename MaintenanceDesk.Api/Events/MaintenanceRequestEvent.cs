namespace MaintenanceDesk.Api.Events;

// Carries only which request changed,  worker fetches current details from the API.
public record MaintenanceRequestEvent(Guid RequestId, MaintenanceRequestEventType Type, DateTimeOffset OccurredAt);
