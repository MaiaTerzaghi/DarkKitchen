using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;

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

    public Product Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }
}
