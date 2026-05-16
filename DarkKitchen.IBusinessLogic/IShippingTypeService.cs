using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingTypeService
{
    List<ShippingTypeResponseDTO> GetAll();
}
