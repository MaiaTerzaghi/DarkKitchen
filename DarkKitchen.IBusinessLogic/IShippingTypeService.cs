using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;


namespace DarkKitchen.IBusinessLogic;

public interface IShippingTypeService
{
    List<ShippingTypeResponseDTO> GetAll();
    ShippingTypeResponseDTO GetById(int id);
    ShippingTypeResponseDTO Create(CreateShippingTypeRequestDTO request);
    ShippingTypeResponseDTO Update(int id, UpdateShippingTypeRequestDTO request);
}
