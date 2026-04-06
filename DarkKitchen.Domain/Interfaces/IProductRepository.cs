using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces;

public interface IProductRepository
{
    List<Product> GetAll(string? name, string? category, string? line);
}
