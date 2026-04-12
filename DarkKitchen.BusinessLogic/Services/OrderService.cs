using DarkKitchen.BusinessLogic.Shipping;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionRepository promotionRepository,
    IUserRepository userRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;

    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IUserRepository _userRepository = userRepository;

    private const double Iva = 0.22;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request)
    {
        var client = _userRepository.GetById(request.ClientId)
            ?? throw new ArgumentException($"Cliente con id {request.ClientId} no encontrado.");

        if(request.Items == null || request.Items.Count == 0)
        {
            throw new ArgumentException("El pedido debe tener al menos un producto.");
        }

        // Guardo en la variable deliveryType el tipo de delivery pero convertido en string
        if (!Enum.TryParse<DeliveryType>(request.DeliveryType, out var deliveryType))
        {
            throw new ArgumentException($"Tipo de entrega '{request.DeliveryType}' no válido.");
        }

        var items = request.Items.Select(i =>
        {
            var product = _productRepository.GetById(i.ProductId) ?? throw new ArgumentException($"Producto con id {i.ProductId} no encontrado.");
            return new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Product = product
            };
        }).ToList();

        var subtotal = items.Sum(i => i.Product.Price * i.Quantity);

        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);

        var discountedSubtotal = ApplyPromotions(subtotal, items, promotions);

        var shippingCost = CalculateShipping(deliveryType);

        var total = (discountedSubtotal * (1 + Iva)) + shippingCost;

        var order = new Order
        {
            ClientId = request.ClientId,
            DeliveryType = deliveryType,
            Status = "Pending",
            Street = request.Address.Street,
            DoorNumber = request.Address.DoorNumber,
            Apartment = request.Address.Apartment,
            Items = items,
            Date = DateTime.Now,
        };

        var saved = _orderRepository.Save(order);

        return new CreateOrderResponseDTO
        {
            ClientId = request.ClientId,
            OrderId = saved.Id,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = Math.Round(total, 2)
        };
    }

    private static double ApplyPromotions(double subtotal, List<OrderItem> items, List<Promotion> promotions)
    {
        double discount = 0;

        foreach(var promotion in promotions)
        {
            var applies = items.Any(i => promotion.Products.Any(p => p.Id == i.ProductId));

            if(applies)
            {
                discount += subtotal * (double)(promotion.DiscountPercentage / 100);
            }
        }

        return subtotal - discount;
    }

    // Implemento Strategy
    private static double CalculateShipping(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => new ExpressShipping().CalculateCost(),
            DeliveryType.Standard => new StandardShipping().CalculateCost(),
            _ => throw new ArgumentException($"Tipo de entrega '{deliveryType}' no disponible.")
        };
    }

    public List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request)
    {
        var orders = _orderRepository.GetClientOrders(request);
        return orders.Select(o => new GetClientOrdersResponseDTO
        {
            OrderId = o.Id,
            ClientId = o.ClientId,
            Status = o.Status,
            Total = o.Items.Sum(i => i.Product.Price * i.Quantity),
            ItemCount = o.Items.Sum(i => i.Quantity)
        }).ToList();
    }

    public List<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request)
    {
        var orders = _orderRepository.GetOrders(request);

        return orders.Select(order => new GetOrdersResponseDTO
        {
            OrderId = order.Id,
            ClientName = order.ClientId.ToString(),
            Date = order.Date,
            Status = order.Status,
            Items = order.Items.Select(item => new OrderItemResponseDTO
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity
            }).ToList()
        }).ToList();
    }
}
