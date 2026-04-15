using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository
{
    List<Product> GetAll(string? name, string? category, string? line);

    Product? GetById(int id);

    Product Add(Product product);
    Product Update(Product product);
    List<Product> GetManage(GetProductsManageRequestDTO request);
}
