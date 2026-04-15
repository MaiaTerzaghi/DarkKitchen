namespace DarkKitchen.IDataAccess;

public interface IRepository<TEntity>
    where TEntity : class
{
    TEntity Add(TEntity entity);
}
