using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IProductService
{
    List<ProductResponseDTO> GetAll(string? name, string? category, string? line, int page = 1, int pageSize = 20);
    ProductResponseDTO CreateProduct(CreateProductRequestDTO request, string responsibleUser);
    ProductResponseDTO UpdateProduct(int id, UpdateProductRequestDTO request);
    List<ProductResponseDTO> GetManage(GetProductsManageRequestDTO request);
}
