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
    OrderDetailResponseDTO GetOrderDetail(int orderId);
    List<TopProductResponseDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo);
    SalesReportWithTotalDTO GetSalesReport(int page, int pageSize);
    OrderPreviewResponseDTO PreviewOrder(List<OrderItemRequestDTO> items, string shippingTypeName);
    UpdateOrderStatusResponseDTO ChangeStatus(int orderId, OrderStatus newStatus, string responsibleUser);
}
