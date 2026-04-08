using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Args.Output;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionRepository promotionRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;

    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    private const double Iva = 0.22;
    private const double ExpressShipping = 50.0;
    private const double StandardShipping = 20.0;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            throw new ArgumentException("El pedido debe tener al menos un producto.");
        }

        var items = request.Items.Select(i =>
        {
            var product = _productRepository.GetById(i.ProductId);
            return new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Product = product
            };
        }).ToList();

        var subtotal = items.Sum(i => i.Product.Price * i.Quantity);

        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);
        double discount = 0;
        foreach (var promo in promotions)
        {
            if (items.Any(i => promo.Products.Any(p => p.Id == i.ProductId)))
            {
                discount += subtotal * ((double)promo.DiscountPercentage / 100);
            }
        }

        var discountedSubtotal = subtotal - discount;

        var shippingCost = request.DeliveryType == "Express" ? ExpressShipping : StandardShipping;

        var total = (discountedSubtotal * (1 + Iva)) + shippingCost;

        var order = new Order
        {
            ClientId = request.ClientId,
            DeliveryType = request.DeliveryType,
            Status = "Pending",
            Street = request.Address.Street,
            DoorNumber = request.Address.DoorNumber,
            Apartment = request.Address.Apartment,
            Items = items
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
}
