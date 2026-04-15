using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class
{
#pragma warning disable CA1823
    private readonly DbContext _context = context;
    private readonly DbSet<TEntity> _entities = context.Set<TEntity>();
#pragma warning restore CA1823
    public TEntity Add(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
