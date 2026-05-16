using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class ShippingTypeService(IRepository<ShippingType> shippingTypeRepository) : IShippingTypeService
{
    private readonly IRepository<ShippingType> _shippingTypeRepository = shippingTypeRepository;

    public List<ShippingTypeResponseDTO> GetAll()
    {
        var shippingTypes = _shippingTypeRepository.GetAll();

        return shippingTypes.Select(st => new ShippingTypeResponseDTO
        {
            Id = st.Id,
            Name = st.Name,
            Cost = st.Cost
        }).ToList();
    }
}
