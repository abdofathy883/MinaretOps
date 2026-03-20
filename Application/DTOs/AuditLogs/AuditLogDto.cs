namespace Application.DTOs.AuditLogs
{
    public record AuditLogDto(
    int Id,
    string EntityName,
    string Action,
    string EntityId,
    string? OldValues,
    string? NewValues,
    string PerformedBy,
    DateTime CreatedAt
);
}
