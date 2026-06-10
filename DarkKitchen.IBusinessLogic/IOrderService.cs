using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IOrderService
{
    CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request, int clientId);
    List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request, int clientId);
    List<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request);
    List<GetOrdersResponseDTO> GetDispatcherOrders();
    UpdateOrderStatusResponseDTO MarkAsPrepared(int orderId);
    OrderDetailResponseDTO GetOrderDetail(int orderId);
    UpdateOrderStatusResponseDTO DeliverOrder(int orderId);
    UpdateOrderStatusResponseDTO CancelOrder(int orderId);
    UpdateOrderStatusResponseDTO MarkAsOnTheWay(int orderId);
    UpdateOrderStatusResponseDTO MarkAsNotDelivered(int orderId);
    UpdateOrderStatusResponseDTO MarkAsDelayed(int orderId);
    List<TopProductResponseDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo);
    SalesReportWithTotalDTO GetSalesReport(int page, int pageSize);
    OrderPreviewResponseDTO PreviewOrder(List<OrderItemRequestDTO> items, string shippingTypeName);
    void ChangeStatus(int orderId, OrderStatus newStatus, string responsibleUser);
}
