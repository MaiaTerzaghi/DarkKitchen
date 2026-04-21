using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository
{
    Order Save(Order order);
    List<Order> GetClientOrders(GetClientOrdersRequestDTO request);
    List<Order> GetOrders(GetOrdersRequestDTO request);
    Order Update(Order order);
    Order? GetOrderById(int orderId);
    List<(Product Product, int Quantity)> GetTopProducts(
    Expression<Func<Order, bool>> predicate,
    int top);
}
