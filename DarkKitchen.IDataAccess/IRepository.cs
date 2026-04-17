using System.Linq.Expressions;
namespace DarkKitchen.IDataAccess;

public interface IRepository<TEntity>
    where TEntity : class
{
    TEntity Add(TEntity entity);
    TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    TEntity Update(TEntity entity);
}
