using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IOrderService
{
    CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request, int clientId);
    PaginatedResponse<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request, UserRole role, int? clientId);
    PaginatedResponse<GetOrdersResponseDTO> GetDispatcherOrders(int page = 1, int pageSize = 20);
    OrderDetailResponseDTO GetOrderDetail(int orderId);
    List<TopProductResponseDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo);
    SalesReportWithTotalDTO GetSalesReport(int page, int pageSize);
    OrderPreviewResponseDTO PreviewOrder(List<OrderItemRequestDTO> items, string shippingTypeName);
    UpdateOrderStatusResponseDTO ChangeStatus(int orderId, OrderStatus target, UserRole role);
}
