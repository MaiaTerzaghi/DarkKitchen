using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository
{
    Order Save(Order order);
    List<Order> GetClientOrders(GetClientOrdersRequestDTO request);
    List<Order> GetOrders(GetOrdersRequestDTO request);
    Order Update(Order order);
    Order? GetOrderById(int orderId);
    List<TopProductResponseDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo, int top);
}
