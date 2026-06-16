using DarkKitchen.Domain.Auditing;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Auditing;

public sealed class AuditLogObserver(IAuditRepository auditRepository) : IAuditObserver
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public void Update(AuditEvent auditEvent)
    {
        var log = new AuditLog
        {
            Timestamp = DateTime.Now,
            EntityName = auditEvent.EntityName,
            EntityId = auditEvent.EntityId,
            Description = auditEvent.Description,
            ResponsibleUser = auditEvent.ResponsibleUser
        };

        _auditRepository.Add(log);
    }
}
