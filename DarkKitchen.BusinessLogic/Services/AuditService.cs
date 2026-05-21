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

        if(request.DateTo is null)
        {
            throw new ArgumentException("El filtro fecha-hora hasta es obligatorio.");
        }

        if(request.DateFrom.Value >= request.DateTo.Value)
        {
            throw new ArgumentException("La fecha-hora desde debe ser menor que la fecha-hora hasta.");
        }

        if(request.EntityName is null)
        {
            throw new ArgumentException("El filtro de entidad es obligatorio.");
        }

        if(request.EntityId is null)
        {
            throw new ArgumentException("El filtro de id de entidad es obligatorio.");
        }

        return [];
    }
}
