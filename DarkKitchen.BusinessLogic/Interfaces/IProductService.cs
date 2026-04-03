using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IProductService
{
    List<Product> GetAll(string? name, string? category, string? line);
}
