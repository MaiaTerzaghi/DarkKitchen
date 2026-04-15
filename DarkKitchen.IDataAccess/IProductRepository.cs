using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository : IRepository<Product>
{
    List<Product> GetAll(string? name, string? category, string? line);

    Product GetById(int id);
}
