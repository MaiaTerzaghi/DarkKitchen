using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;

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
        throw new NotImplementedException();
    }
}
