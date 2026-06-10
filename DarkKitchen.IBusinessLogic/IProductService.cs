using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IProductService
{
    List<ProductResponseDTO> GetProducts(GetProductsManageRequestDTO request, UserRole role);
    ProductResponseDTO CreateProduct(CreateProductRequestDTO request, string responsibleUser);
    ProductResponseDTO UpdateProduct(int id, UpdateProductRequestDTO request, string responsibleUser);
}
