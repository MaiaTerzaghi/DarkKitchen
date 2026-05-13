using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(DarkKitchenContext context)
    : Repository<Order>(context), IOrderRepository
{
    public List<Order> GetClientOrders(
        int clientId,
        OrderStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        return context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ClientId == clientId)
            .Where(o => !status.HasValue || o.Status == status.Value)
            .Where(o => !dateFrom.HasValue || o.Date >= dateFrom.Value)
            .Where(o => !dateTo.HasValue || o.Date <= dateTo.Value)
            .ToList();
    }

    public List<Order> GetOrders(
        DateTime dateFrom,
        DateTime dateTo,
        string? street,
        OrderStatus? status)
    {
        var query = context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Client)
            .Where(o => o.Date >= dateFrom && o.Date <= dateTo);

        if(!string.IsNullOrEmpty(street))
        {
            query = query.Where(o => o.Street.Contains(street));
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return query.ToList();
    }

    public Order? GetOrderById(int orderId)
    {
        return context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.ShippingType)
            .FirstOrDefault(o => o.Id == orderId);
    }

    public List<(Product Product, int Quantity)> GetTopProducts(
        Expression<Func<Order, bool>> predicate,
        int top)
    {
        return context.Orders
            .Where(predicate)
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Product)
            .Select(g => new { Product = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .OrderByDescending(g => g.Quantity)
            .Take(top)
            .AsEnumerable()
            .Select(g => (g.Product, g.Quantity))
            .ToList();
    }

    public List<(int Year, int Month, int ClientId, string ClientName, double Total)> GetSalesReport(
        int page,
        int pageSize)
    {
        return context.Orders
            .GroupBy(o => new { o.Date.Year, o.Date.Month, o.ClientId, ClientName = o.Client.Name + " " + o.Client.LastName })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.ClientId,
                g.Key.ClientName,
                Total = g.Sum(o => o.Total)
            })
            .OrderByDescending(g => g.Year)
            .ThenByDescending(g => g.Month)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsEnumerable()
            .Select(g => (g.Year, g.Month, g.ClientId, g.ClientName, g.Total))
            .ToList();
    }
}
