using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.IDataAccess;

public interface IAuditRepository : IRepository<AuditLog>
{
    (List<AuditLog> Items, int TotalCount) GetByEntity(AuditedEntity entityName, int entityId, DateTime from, DateTime to, int page = 1, int pageSize = 20);
}
