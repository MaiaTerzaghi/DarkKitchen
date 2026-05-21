using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public AuditedEntity EntityName { get; set; }
    public int EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
}
