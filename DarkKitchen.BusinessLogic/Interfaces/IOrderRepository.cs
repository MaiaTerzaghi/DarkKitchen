using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IOrderRepository
{
    Order Save(Order order);

    List<Order> GetOrders(GetOrdersRequestDTO request);
}
