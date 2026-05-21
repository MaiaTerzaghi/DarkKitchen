using DarkKitchen.Domain.Enums;

namespace DarkKitchen.DTOs.Args.In;

public class GetAuditLogsRequestDTO
{
    public AuditedEntity? EntityName { get; set; }
    public int? EntityId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
