namespace DarkKitchen.DTOs.Args.Output;

public class AuditLogResponseDTO
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
}
