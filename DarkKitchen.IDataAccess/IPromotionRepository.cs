using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;
public interface IPromotionRepository : IRepository<Promotion>
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
}
