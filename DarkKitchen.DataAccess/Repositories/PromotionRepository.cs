using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(DarkKitchenContext context)
    : Repository<Promotion>(context), IPromotionRepository
{
    public (List<Promotion> Items, int TotalCount) GetActivePromotions(DateTime? date, string? productLine, string? product, int page = 1, int pageSize = 20)
    {
        var query = context.Promotions
            .Include(p => p.Products)
            .Where(p => !date.HasValue || (p.ValidFrom <= date.Value && p.ValidTo >= date.Value))
            .Where(p => string.IsNullOrEmpty(productLine) || p.ProductLine == productLine)
            .Where(p => string.IsNullOrEmpty(product) || p.Products.Any(pr => pr.Name == product))
            .OrderByDescending(p => p.ValidFrom);

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public Promotion? GetPromotionWithProducts(int promotionId)
    {
        return context.Promotions
            .Include(p => p.Products)
            .FirstOrDefault(p => p.Id == promotionId);
    }
}
