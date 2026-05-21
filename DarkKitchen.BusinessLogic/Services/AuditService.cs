using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Services;

public class AuditService : IAuditService
{
    public List<AuditLogResponseDTO> GetLogs(GetAuditLogsRequestDTO request)
    {
        if(request.DateFrom is null)
        {
            throw new ArgumentException("El filtro fecha-hora desde es obligatorio.");
        }

        return [];
    }
}
