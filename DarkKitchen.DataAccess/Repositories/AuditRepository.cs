using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class AuditLogRepository(DarkKitchenContext context)
    : Repository<AuditLog>(context), IAuditRepository
{
    public (List<AuditLog> Items, int TotalCount) GetByEntity(AuditedEntity entityName, int entityId, DateTime from, DateTime to, int page = 1, int pageSize = 20)
    {
        var query = context.AuditLogs
            .Where(a => a.EntityName == entityName
                && a.EntityId == entityId
                && a.Timestamp >= from
                && a.Timestamp <= to)
            .OrderByDescending(a => a.Timestamp);

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }
}
