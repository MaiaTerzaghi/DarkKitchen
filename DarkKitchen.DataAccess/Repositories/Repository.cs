using System.Linq.Expressions;
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

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities.FirstOrDefault(predicate);
    }

    public TEntity Update(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
