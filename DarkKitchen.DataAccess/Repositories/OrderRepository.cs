using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
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

    public List<Order> GetOrders(GetOrdersRequestDTO request)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Date >= request.DateFrom && o.Date <= request.DateTo);

        if (!string.IsNullOrEmpty(request.Street))
        {
            query = query.Where(o => o.Street.Contains(request.Street));
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(o => o.Status == request.Status);
        }

        return query.ToList();
    }
}
