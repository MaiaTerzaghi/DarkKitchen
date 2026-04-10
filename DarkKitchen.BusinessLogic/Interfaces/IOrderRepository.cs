using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IOrderRepository
{
    Order Save(Order order);
    List<Order> GetClientOrders(GetClientOrdersRequestDTO request);

    List<Order> GetOrders(GetOrdersRequestDTO request);
}
