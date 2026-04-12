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
            .Where(o => string.IsNullOrEmpty(request.Status) || o.Status == request.Status)
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

        if(!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(o => o.Status == request.Status);
        }

        return query.ToList();
    }

    public Order? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Order Update(Order order)
    {
        throw new NotImplementedException();
    }
}
