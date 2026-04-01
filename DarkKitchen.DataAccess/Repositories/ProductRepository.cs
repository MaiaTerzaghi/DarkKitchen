using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(DarkKitchenContext context) : IProductRepository
{
    private readonly DarkKitchenContext _context = context;

    public List<Product> GetAll(string? name, string? category, string? line)
    {
        return _context.Products.ToList();
    }
}
