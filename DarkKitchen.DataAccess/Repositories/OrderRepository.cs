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

    public List<Order> GetClientOrders(GetClientOrdersRequestDTO request)
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ClientId == request.ClientId)
            .Where(o => string.IsNullOrEmpty(request.Status) || o.Status == request.Status)
            .Where(o => !request.DateFrom.HasValue || o.Date >= request.DateFrom.Value)
            .Where(o => !request.DateTo.HasValue || o.Date <= request.DateTo.Value)
            .ToList();
    }
}
