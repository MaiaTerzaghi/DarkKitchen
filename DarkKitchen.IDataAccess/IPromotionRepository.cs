using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;
public interface IPromotionRepository : IRepository<Promotion>
{
    (List<Promotion> Items, int TotalCount) GetActivePromotions(DateTime? date, string? productLine, string? product, int page = 1, int pageSize = 20);

    Promotion? GetPromotionWithProducts(int promotionId);
}
