using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Args.Output;
using DarkKitchen.BusinessLogic.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

    public class OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository) : IOrderService
    {
        // TDD Red: se suprime el warning de campo no utilizado temporalmente.
        // Los campos serán utilizados en el paso Green cuando se implemente CreateOrder.
    #pragma warning disable CA1823, CS9113
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IProductRepository _productRepository = productRepository;
    #pragma warning restore CA1823, CS9113

        public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request)
        {
            throw new NotImplementedException();
        }
}
