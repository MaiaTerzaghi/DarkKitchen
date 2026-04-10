using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;
public interface IPromotionRepository
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
}
