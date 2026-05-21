using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class AuditLogRepository(DarkKitchenContext context)
    : Repository<AuditLog>(context), IAuditRepository
{
    public List<AuditLog> GetByEntity(AuditedEntity entityName, int entityId, DateTime from, DateTime to)
    {
        return context.AuditLogs
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .ToList();
    }
}
