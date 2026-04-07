using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        return _promotionRepository.GetActivePromotions(date, productLine, product);
    }
}
