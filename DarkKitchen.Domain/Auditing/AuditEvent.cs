using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Auditing;

public sealed class AuditEvent
{
    public AuditedEntity EntityName { get; init; }
    public int EntityId { get; init; }
    public string ResponsibleUser { get; init; } = string.Empty;
}
