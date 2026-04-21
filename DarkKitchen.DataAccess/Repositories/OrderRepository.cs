using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(DarkKitchenContext context) : IOrderRepository
{
    private readonly DarkKitchenContext _context = context;

    public Order Save(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }

    public List<Order> GetClientOrders(GetClientOrdersRequestDTO request)
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ClientId == request.ClientId)
            .Where(o => !request.Status.HasValue || o.Status == request.Status.Value)
            .Where(o => !request.DateFrom.HasValue || o.Date >= request.DateFrom.Value)
            .Where(o => !request.DateTo.HasValue || o.Date <= request.DateTo.Value)
            .ToList();
    }

    public List<Order> GetOrders(GetOrdersRequestDTO request)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Date >= request.DateFrom && o.Date <= request.DateTo);

        if(!string.IsNullOrEmpty(request.Street))
        {
            query = query.Where(o => o.Street.Contains(request.Street));
        }

        if(request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        return query.ToList();
    }

    public Order Update(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
        return order;
    }

    public Order? GetOrderById(int orderId)
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(o => o.Id == orderId);
    }

    public List<(Product Product, int Quantity)> GetTopProducts(
    Expression<Func<Order, bool>> predicate,
    int top)
    {
        return _context.Orders
            .Where(predicate)
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Product)
            .Select(g => new { Product = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .AsEnumerable()
            .Select(g => (g.Product, g.Quantity))
            .ToList();
    }
}
