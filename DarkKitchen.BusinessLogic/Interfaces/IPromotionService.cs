using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IPromotionService
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
}
