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
        _entities.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public List<TEntity> GetAll(
    Expression<Func<TEntity, bool>>? predicate = null,
    Expression<Func<TEntity, object>>? orderBy = null,
    bool descending = false,
    int page = 1,
    int pageSize = 20)
    {
        var query = _entities.AsQueryable();

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
