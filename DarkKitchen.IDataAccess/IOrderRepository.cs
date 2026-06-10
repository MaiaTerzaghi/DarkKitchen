using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
namespace DarkKitchen.IDataAccess;

public interface IOrderRepository : IRepository<Order>
{
    (List<Order> Items, int TotalCount) GetOrders(
        int? clientId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? street,
        OrderStatus? status,
        int page = 1,
        int pageSize = 20);
    Order? GetOrderById(int orderId);
    List<(Product Product, int Quantity)> GetTopProducts(
    Expression<Func<Order, bool>> predicate,
    int top);
    (List<(int Year, int Month, int ClientId, string ClientName, double Total)> Items, int TotalCount) GetSalesReport(int page, int pageSize);
    List<Order> GetDispatcherOrders();
}
