using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(DarkKitchenContext context) : IPromotionRepository
{
    private readonly DarkKitchenContext _context = context;

    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        return _context.Promotions
            .Where(p => !date.HasValue || (p.ValidFrom <= date.Value && p.ValidTo >= date.Value))
            .Where(p => string.IsNullOrEmpty(productLine) || p.ProductLine == productLine)
            .ToList();
    }
}
