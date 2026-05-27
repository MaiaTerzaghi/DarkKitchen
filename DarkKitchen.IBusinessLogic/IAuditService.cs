using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;

public interface IAuditService
{
    List<AuditLogResponseDTO> GetLogs(GetAuditLogsRequestDTO request);
}
