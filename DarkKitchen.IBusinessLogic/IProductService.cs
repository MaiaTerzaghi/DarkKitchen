using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IProductService
{
    List<Product> GetAll(string? name, string? category, string? line);
    ProductResponseDTO CreateProduct(CreateProductRequestDTO request);
}
