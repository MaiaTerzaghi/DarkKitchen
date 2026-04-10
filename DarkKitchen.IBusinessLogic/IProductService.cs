using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IBusinessLogic;
public interface IProductService
{
    List<Product> GetAll(string? name, string? category, string? line);
}
