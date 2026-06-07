using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Validators;
using DarkKitchen.DTOs.Args.In;
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

    public ShippingTypeResponseDTO GetById(int id)
    {
        var shippingType = _shippingTypeRepository.Get(st => st.Id == id)
            ?? throw new NotFoundException($"Tipo de envío con id {id} no encontrado.");

        return new ShippingTypeResponseDTO
        {
            Id = shippingType.Id,
            Name = shippingType.Name,
            Cost = shippingType.Cost
        };
    }

    public ShippingTypeResponseDTO Create(CreateShippingTypeRequestDTO request)
    {
        ShippingTypeValidator.ValidateName(request.Name);
        ShippingTypeValidator.ValidateCost(request.Cost);

        var existing = _shippingTypeRepository.Get(st => st.Name == request.Name);
        if(existing != null)
        {
            throw new ConflictException($"Ya existe un tipo de envío con el nombre '{request.Name}'.");
        }

        var shippingType = new ShippingType
        {
            Name = request.Name,
            Cost = request.Cost
        };

        var saved = _shippingTypeRepository.Add(shippingType);

        return new ShippingTypeResponseDTO
        {
            Id = saved.Id,
            Name = saved.Name,
            Cost = saved.Cost
        };
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public ShippingTypeResponseDTO Update(int id, UpdateShippingTypeRequestDTO request)
    {
        ShippingTypeValidator.ValidateName(request.Name);
        ShippingTypeValidator.ValidateCost(request.Cost);

        var shippingType = _shippingTypeRepository.Get(st => st.Id == id)
            ?? throw new NotFoundException($"Tipo de envío con id {id} no encontrado.");

        var duplicate = _shippingTypeRepository.Get(st => st.Name == request.Name && st.Id != id);
        if(duplicate != null)
        {
            throw new ConflictException($"Ya existe un tipo de envío con el nombre '{request.Name}'.");
        }

        shippingType.Name = request.Name;
        shippingType.Cost = request.Cost;

        var updated = _shippingTypeRepository.Update(shippingType);

        return new ShippingTypeResponseDTO
        {
            Id = updated.Id,
            Name = updated.Name,
            Cost = updated.Cost
        };
    }
}
