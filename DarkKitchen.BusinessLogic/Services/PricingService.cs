using DarkKitchen.BusinessLogic.Shipping;
using DarkKitchen.Domain;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class PricingService(
    IRepository<Product> productRepository,
    IPromotionRepository promotionRepository) : IPricingService
{
    private readonly IRepository<Product> _productRepository = productRepository;
    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    public PricingResult CalculateOrderPricing(List<OrderItemRequestDTO> items, DeliveryType deliveryType)
    {
        var itemsWithProducts = BuildOrderItems(items);
        var orderItems = itemsWithProducts.Select(i => i.Item).ToList();
        var subtotal = itemsWithProducts.Sum(i => i.Product.Price * i.Item.Quantity);
        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);
        var discountedSubtotal = ApplyPromotions(subtotal, itemsWithProducts, promotions);
        var discount = subtotal - discountedSubtotal;
        var shippingCost = CalculateShipping(deliveryType);
        var vat = Math.Round(discountedSubtotal * AppConstants.Vat, 2);
        var total = Math.Round((discountedSubtotal * (1 + AppConstants.Vat)) + shippingCost, 2);

        return new PricingResult
        {
            Items = orderItems,
            Subtotal = subtotal,
            Discount = discount,
            ShippingCost = shippingCost,
            Vat = vat,
            Total = total
        };
    }

    private List<(OrderItem Item, Product Product)> BuildOrderItems(List<OrderItemRequestDTO> items)
    {
        return items.Select(i =>
        {
            var product = _productRepository.Get(p => p.Id == i.ProductId)
                ?? throw new NotFoundException($"Producto con id {i.ProductId} no encontrado.");

            if(!product.IsActive)
            {
                throw new ArgumentException($"El producto {product.Name} está inactivo.");
            }

            return (Item: new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity, UnitPrice = product.Price }, Product: product);
        }).ToList();
    }

    private static double ApplyPromotions(double subtotal, List<(OrderItem Item, Product Product)> itemsWithProducts, List<Promotion> promotions)
    {
        double discount = 0;

        foreach(var (item, product) in itemsWithProducts)
        {
            var bestPromotion = promotions
                .Where(p => p.Products.Any(prod => prod.Id == item.ProductId))
                .MaxBy(p => p.DiscountPercentage);

            if(bestPromotion != null)
            {
                var itemSubtotal = product.Price * item.Quantity;
                discount += itemSubtotal * (double)(bestPromotion.DiscountPercentage / 100);
            }
        }

        return subtotal - discount;
    }

    private static double CalculateShipping(DeliveryType deliveryType)
    {
        IShippingStrategy shippingStrategy = deliveryType switch
        {
            DeliveryType.Express => new ExpressShipping(),
            DeliveryType.Standard => new StandardShipping(),
            _ => throw new ArgumentException("Tipo de entrega no válido")
        };

        return shippingStrategy.CalculateCost();
    }
}
