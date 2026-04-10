using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IBusinessLogic;
public interface IPromotionService
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
}
