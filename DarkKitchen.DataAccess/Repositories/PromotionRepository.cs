using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(DarkKitchenContext context)
    : Repository<Promotion>(context), IPromotionRepository
{
    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        return context.Promotions
            .Include(p => p.Products)
            .Where(p => !date.HasValue || (p.ValidFrom <= date.Value && p.ValidTo >= date.Value))
            .Where(p => string.IsNullOrEmpty(productLine) || p.ProductLine == productLine)
            .Where(p => string.IsNullOrEmpty(product) || p.Products.Any(pr => pr.Name == product))
            .ToList();
    }

    public bool ProductExistsInPromotion(int promotionId, int productId)
    {
        return context.Promotions
            .Any(p => p.Id == promotionId
                && p.Products.Any(pr => pr.Id == productId));
    }

    public Promotion? GetPromotionWithProducts(int promotionId)
    {
        return context.Promotions
            .Include(p => p.Products)
            .FirstOrDefault(p => p.Id == promotionId);
    }
}
