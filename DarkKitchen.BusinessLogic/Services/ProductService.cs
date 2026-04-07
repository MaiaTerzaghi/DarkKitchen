using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public List<Product> GetAll(string? name, string? category, string? line)
    {
        return _productRepository.GetAll(name, category, line);
    }
}
