using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService : IPromotionService
{
    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        throw new NotImplementedException();
    }
}
