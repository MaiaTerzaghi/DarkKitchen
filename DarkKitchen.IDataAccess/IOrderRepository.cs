using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository
{
    Order Save(Order order);
    List<Order> GetClientOrders(GetClientOrdersRequestDTO request);
    List<Order> GetOrders(GetOrdersRequestDTO request);

    Order? GetById(int id);
    Order Update(Order order);
    Order? GetOrderById(int orderId);
}
