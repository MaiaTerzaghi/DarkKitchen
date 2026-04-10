using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Args.Output;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IOrderService
{
    CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request);
    List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request);
    List<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request);
}
