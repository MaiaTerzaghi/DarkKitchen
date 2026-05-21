using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.IDataAccess;

public interface IAuditRepository : IRepository<AuditLog>
{
    List<AuditLog> GetByEntity(AuditedEntity entityName, int entityId, DateTime from, DateTime to);
}
