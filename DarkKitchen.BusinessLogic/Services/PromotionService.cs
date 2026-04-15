using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        return _promotionRepository.GetActivePromotions(date, productLine, product);
    }

    public PromotionResponseDTO CreatePromotion(CreatePromotionRequestDTO request)
    {
        if(string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre de la promoción no puede estar vacío.");
        }

        if(request.DiscountPercentage <= 0 || request.DiscountPercentage > 100)
        {
            throw new ArgumentException("El porcentaje de descuento debe ser mayor que 0 y menor o igual a 100.");
        }

        if(request.ValidFrom > request.ValidTo)
        {
            throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
        }

        var promotion = new Promotion
        {
            Name = request.Name,
            DiscountPercentage = request.DiscountPercentage,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };

        var saved = _promotionRepository.Add(promotion);

        return new PromotionResponseDTO
        {
            Id = saved.Id,
            Name = saved.Name,
            DiscountPercentage = saved.DiscountPercentage,
            ValidFrom = saved.ValidFrom,
            ValidTo = saved.ValidTo
        };
    }

    public PromotionResponseDTO UpdatePromotion(int id, UpdatePromotionRequestDTO request)
    {
        if(string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre de la promoción no puede estar vacío.");
        }

        if(request.DiscountPercentage <= 0 || request.DiscountPercentage > 100)
        {
            throw new ArgumentException("El porcentaje de descuento debe ser mayor que 0 y menor o igual a 100.");
        }

        if(request.ValidFrom > request.ValidTo)
        {
            throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
        }

        var promotion = _promotionRepository.Get(p => p.Id == id)
            ?? throw new ArgumentException($"Promoción con id {id} no encontrada.");

        promotion.Name = request.Name;
        promotion.DiscountPercentage = request.DiscountPercentage;
        promotion.ValidFrom = request.ValidFrom;
        promotion.ValidTo = request.ValidTo;

        var updated = _promotionRepository.Update(promotion);

        return new PromotionResponseDTO
        {
            Id = updated.Id,
            Name = updated.Name,
            DiscountPercentage = updated.DiscountPercentage,
            ValidFrom = updated.ValidFrom,
            ValidTo = updated.ValidTo
        };
    }
}
