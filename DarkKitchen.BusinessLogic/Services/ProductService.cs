using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IRepository<Product> productRepository) : IProductService
{
    private readonly IRepository<Product> _productRepository = productRepository;

    public List<ProductResponseDTO> GetAll(string? name, string? category, string? line)
    {
        var products = _productRepository.GetAll(
        predicate: p =>
            (string.IsNullOrEmpty(name) || p.Name.Contains(name)) &&
            (string.IsNullOrEmpty(category) || p.Category == category) &&
            (string.IsNullOrEmpty(line) || p.CommercialLine == line));

        return products.Select(p => new ProductResponseDTO
        {
            Code = p.Code,
            Name = p.Name,
            Price = p.Price,
            CommercialLine = p.CommercialLine,
            Category = p.Category,
            Images = p.Images
        }).ToList();
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
        var product = _productRepository.Get(p => p.Id == id)
            ?? throw new NotFoundException($"Producto con id {id} no encontrado.");

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

    public List<ProductResponseDTO> GetManage(GetProductsManageRequestDTO request)
    {
        var products = _productRepository.GetAll(
            predicate: p =>
                (string.IsNullOrEmpty(request.Name) || p.Name.Contains(request.Name)) &&
                (string.IsNullOrEmpty(request.Description) || p.Description.Contains(request.Description)) &&
                (string.IsNullOrEmpty(request.Category) || p.Category.Contains(request.Category)) &&
                (string.IsNullOrEmpty(request.CommercialLine) || p.CommercialLine.Contains(request.CommercialLine)) &&
                (!request.IsActive.HasValue || p.IsActive == request.IsActive.Value) &&
                (!request.PriceMin.HasValue || p.Price >= request.PriceMin.Value) &&
                (!request.PriceMax.HasValue || p.Price <= request.PriceMax.Value));

        return products.Select(p => new ProductResponseDTO
        {
            Code = p.Code,
            Name = p.Name,
            Price = p.Price,
            CommercialLine = p.CommercialLine,
            Category = p.Category,
            Images = p.Images
        }).ToList();
    }
}
