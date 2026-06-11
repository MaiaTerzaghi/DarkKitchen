using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Models;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(DarkKitchenContext context)
    : Repository<Order>(context), IOrderRepository
{
    public (List<Order> Items, int TotalCount) GetOrders(
        int? clientId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? street,
        OrderStatus? status,
        int page = 1,
        int pageSize = 20)
    {
        var query = context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Client)
            .Where(o => !clientId.HasValue || o.ClientId == clientId.Value)
            .Where(o => !dateFrom.HasValue || o.Date.Date >= dateFrom.Value.Date)
            .Where(o => !dateTo.HasValue || o.Date.Date <= dateTo.Value.Date)
            .Where(o => string.IsNullOrEmpty(street) || o.Street.Contains(street))
            .Where(o => !status.HasValue || o.Status == status.Value)
            .OrderByDescending(o => o.Date);
        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
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

    public (List<SalesReportItem> Items, int TotalCount) GetSalesReport(
        int page,
        int pageSize)
    {
        var query = context.Orders
            .GroupBy(o => new { o.Date.Year, o.Date.Month, o.ClientId, ClientName = o.Client.Name + " " + o.Client.LastName })
            .Select(g => new SalesReportItem
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                ClientId = g.Key.ClientId,
                ClientName = g.Key.ClientName,
                Total = g.Sum(o => o.Total)
            })
            .OrderByDescending(g => g.Year)
            .ThenByDescending(g => g.Month);

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public List<Order> GetDispatcherOrders()
    {
        return context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Client)
            .ToList();
    }
}
