using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
namespace DarkKitchen.IDataAccess;

public interface IOrderRepository : IRepository<Order>
{
    List<Order> GetClientOrders(
        int clientId,
        OrderStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo);

    List<Order> GetOrders(
        DateTime dateFrom,
        DateTime dateTo,
        string? street,
        OrderStatus? status);
    Order? GetOrderById(int orderId);
    List<(Product Product, int Quantity)> GetTopProducts(
    Expression<Func<Order, bool>> predicate,
    int top);
    (List<(int Year, int Month, int ClientId, string ClientName, double Total)> Items, int TotalCount) GetSalesReport(int page, int pageSize);
    List<Order> GetDispatcherOrders();
}
