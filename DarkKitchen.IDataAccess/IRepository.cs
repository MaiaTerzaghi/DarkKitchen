using System.Linq.Expressions;
namespace DarkKitchen.IDataAccess;

public interface IRepository<TEntity>
    where TEntity : class
{
    TEntity Add(TEntity entity);
    TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    TEntity Update(TEntity entity);
    List<TEntity> GetAll(
    Expression<Func<TEntity, bool>>? predicate = null,
    Expression<Func<TEntity, object>>? orderBy = null,
    bool descending = false,
    int page = 1,
    int pageSize = 20);
}
