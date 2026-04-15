using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbContext _context = context;
    private readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    public TEntity Add(TEntity entity)
    {
        _entities.Add(entity);
        _context.SaveChanges();
        return entity;
    }
}
