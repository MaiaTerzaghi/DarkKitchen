using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(DarkKitchenContext context) : IProductRepository
{
    private readonly DarkKitchenContext _context = context;

    public List<Product> GetAll(string? name, string? category, string? line)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        return query.ToList();
}
}
