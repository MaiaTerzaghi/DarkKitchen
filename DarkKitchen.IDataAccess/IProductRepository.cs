using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository
{
    List<Product> GetAll(string? name, string? category, string? line);

    Product GetById(int id);
}
