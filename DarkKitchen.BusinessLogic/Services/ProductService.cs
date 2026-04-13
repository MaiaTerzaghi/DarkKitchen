using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public List<Product> GetAll(string? name, string? category, string? line)
    {
        return _productRepository.GetAll(name, category, line);
    }

    public ProductResponseDTO CreateProduct(CreateProductRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
