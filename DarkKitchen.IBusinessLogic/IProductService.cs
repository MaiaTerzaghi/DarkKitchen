using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IProductService
{
    List<ProductResponseDTO> GetAll(string? name, string? category, string? line);
    ProductResponseDTO CreateProduct(CreateProductRequestDTO request);
    ProductResponseDTO UpdateProduct(int id, UpdateProductRequestDTO request);
    List<ProductResponseDTO> GetManage(GetProductsManageRequestDTO request);
}
