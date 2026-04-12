using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IOrderService
{
    CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request);
    List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request);
    List<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request);
    UpdateOrderStatusResponseDTO MarkAsPrepared(int orderId);
    OrderDetailResponseDTO GetOrderDetail(int orderId);
}
