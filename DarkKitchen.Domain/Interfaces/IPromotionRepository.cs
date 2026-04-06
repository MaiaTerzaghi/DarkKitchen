using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces;

public interface IPromotionRepository
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
}
