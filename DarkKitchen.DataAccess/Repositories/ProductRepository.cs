using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(DarkKitchenContext context) : IProductRepository
{
    private readonly DarkKitchenContext _context = context;

    public List<Product> GetAll(string? name, string? category, string? line)
    {
        var query = _context.Products.AsQueryable(); // Construye la consulta sin ejecutarla, permitiendo agregar filtros dinámicamente en una sola query SQL en lugar de traer todos los datos a memoria

        if(!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if(!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if(!string.IsNullOrEmpty(line))
        {
            query = query.Where(p => p.CommercialLine == line);
        }

        return query.ToList();
    }

    public Product GetById(int id)
    {
        return _context.Products.FirstOrDefault(p => p.Id == id)!;
    }
}
