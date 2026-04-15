using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
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

    public Product Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
        return product;
    }

    public List<Product> GetManage(GetProductsManageRequestDTO request)
    {
        var query = _context.Products.AsQueryable();

        if(!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(p => p.Name.Contains(request.Name));
        }

        if(!string.IsNullOrEmpty(request.Description))
        {
            query = query.Where(p => p.Description.Contains(request.Description));
        }

        if(!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(p => p.Category.Contains(request.Category));
        }

        if(!string.IsNullOrEmpty(request.CommercialLine))
        {
            query = query.Where(p => p.CommercialLine.Contains(request.CommercialLine));
        }

        if(request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        if(request.PriceMin.HasValue)
        {
            query = query.Where(p => p.Price >= request.PriceMin.Value);
        }

        if(request.PriceMax.HasValue)
        {
            query = query.Where(p => p.Price <= request.PriceMax.Value);
        }

        return query.ToList();
    }
}
