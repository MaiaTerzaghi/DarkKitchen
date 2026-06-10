using DarkKitchen.Domain.Auditing;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IRepository<Product> productRepository, IAuditSubject audit) : IProductService
{
    private readonly IRepository<Product> _productRepository = productRepository;
    private readonly IAuditSubject _audit = audit;

    public List<ProductResponseDTO> GetProducts(GetProductsManageRequestDTO request, UserRole role)
    {
        if(role == UserRole.Client)
        {
            request.IsActive = true;
        }

        var products = _productRepository.GetAll(
            predicate: p =>
                (string.IsNullOrEmpty(request.Name) || p.Name.Contains(request.Name)) &&
                (string.IsNullOrEmpty(request.Description) || p.Description.Contains(request.Description)) &&
                (string.IsNullOrEmpty(request.Category) || p.Category.Contains(request.Category)) &&
                (string.IsNullOrEmpty(request.CommercialLine) || p.CommercialLine.Contains(request.CommercialLine)) &&
                (!request.IsActive.HasValue || p.IsActive == request.IsActive.Value) &&
                (!request.PriceMin.HasValue || p.Price >= request.PriceMin.Value) &&
                (!request.PriceMax.HasValue || p.Price <= request.PriceMax.Value),
            page: request.Page,
            pageSize: request.PageSize);

        return products.Select(MapToDTO).ToList();
    }

    public ProductResponseDTO CreateProduct(CreateProductRequestDTO request, string responsibleUser)
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

        _audit.Notify(new AuditEvent
        {
            EntityName = AuditedEntity.Product,
            EntityId = saved.Id,
            Description = $"Alta de producto '{saved.Name}' ({saved.Code}).",
            ResponsibleUser = responsibleUser
        });

        return MapToDTO(saved);
    }

    public ProductResponseDTO UpdateProduct(int id, UpdateProductRequestDTO request, string responsibleUser)
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

        _audit.Notify(new AuditEvent
        {
            EntityName = AuditedEntity.Product,
            EntityId = updated.Id,
            Description = $"Modificación de producto '{updated.Name}' ({updated.Code}).",
            ResponsibleUser = responsibleUser
        });

        return MapToDTO(updated);
    }

    private static ProductResponseDTO MapToDTO(Product p)
    {
        return new ProductResponseDTO
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CommercialLine = p.CommercialLine,
            Category = p.Category,
            Images = p.Images,
            IsActive = p.IsActive
        };
    }
}
