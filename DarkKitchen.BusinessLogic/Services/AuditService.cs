using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuditService(IAuditRepository auditRepository) : IAuditService
{
    private readonly IAuditRepository _auditRepository = auditRepository;
    public List<AuditLogResponseDTO> GetLogs(GetAuditLogsRequestDTO request)
    {
        ValidateFilters(request);

        var logs = _auditRepository.GetByEntity(
            request.EntityName!.Value,
            request.EntityId!.Value,
            request.DateFrom!.Value,
            request.DateTo!.Value);

        return logs.Select(log => new AuditLogResponseDTO()).ToList();
    }

    private static void ValidateFilters(GetAuditLogsRequestDTO request)
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
    }
}
