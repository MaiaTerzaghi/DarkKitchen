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
        var product = _productRepository.GetById(id)
            ?? throw new ArgumentException($"Producto con id {id} no encontrado.");

        product.Code = request.Code;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CommercialLine = request.CommercialLine;
        product.Category = request.Category;
        product.Images = request.Images;
        product.IsActive = request.IsActive;

        var updated = _productRepository.Update(product);

        return new ProductResponseDTO
        {
            Code = updated.Code,
            Name = updated.Name,
            Price = updated.Price,
            CommercialLine = updated.CommercialLine,
            Category = updated.Category,
            Images = updated.Images
        };
    }
}
