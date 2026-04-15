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
        var product = new Product
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CommercialLine = request.CommercialLine,
            Category = request.Category,
            Images = request.Images,
            IsActive = true
        };

        var saved = _productRepository.Add(product);

        return new ProductResponseDTO
        {
            Code = saved.Code,
            Name = saved.Name,
            Price = saved.Price,
            CommercialLine = saved.CommercialLine,
            Category = saved.Category,
            Images = saved.Images
        };
    }

    public ProductResponseDTO UpdateProduct(int id, UpdateProductRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
