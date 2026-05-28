using DarkKitchen.Domain;
using DarkKitchen.Domain.Entities;
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

    public PricingResult CalculateOrderPricing(List<OrderItemRequestDTO> items, ShippingType shippingType)
    {
        var itemsWithProducts = BuildOrderItems(items);
        var orderItems = itemsWithProducts.Select(i => i.Item).ToList();
        var subtotal = CalculateSubtotal(itemsWithProducts);
        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);
        var discount = CalculateTotalDiscount(itemsWithProducts, promotions);
        var totals = CalculateTotals(subtotal, discount, shippingType.Cost);

        return new PricingResult
        {
            Items = orderItems,
            Subtotal = subtotal,
            Discount = discount,
            ShippingCost = shippingType.Cost,
            Vat = totals.Vat,
            Total = totals.Total,
        };
    }

    public OrderPreviewResponseDTO PreviewOrderPricing(List<OrderItemRequestDTO> items, ShippingType shippingType)
    {
        var itemsWithProducts = BuildOrderItems(items);
        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);
        var subtotal = CalculateSubtotal(itemsWithProducts);

        var previewItems = itemsWithProducts
            .Select(i => BuildPreviewItem(i.Item, i.Product, promotions))
            .ToList();

        var discount = previewItems.Sum(i => (i.UnitPrice - i.DiscountedUnitPrice) * i.Quantity);
        discount = Math.Round(discount, 2);
        var totals = CalculateTotals(subtotal, discount, shippingType.Cost);

        return new OrderPreviewResponseDTO
        {
            Items = previewItems,
            Subtotal = subtotal,
            Discount = discount,
            Vat = totals.Vat,
            ShippingCost = shippingType.Cost,
            Total = totals.Total,
        };
    }

    private static Promotion? GetBestPromotion(int productId, List<Promotion> promotions)
    {
        return promotions
            .Where(p => p.Products.Any(prod => prod.Id == productId))
            .MaxBy(p => p.DiscountPercentage);
    }

    private static double GetDiscountPercentage(int productId, List<Promotion> promotions)
    {
        var best = GetBestPromotion(productId, promotions);
        return best != null ? (double)best.DiscountPercentage : 0;
    }

    private static OrderPreviewItemDTO BuildPreviewItem(OrderItem item, Product product, List<Promotion> promotions)
    {
        var discountPct = GetDiscountPercentage(item.ProductId, promotions);
        var discountedUnitPrice = Math.Round(product.Price * (1 - (discountPct / 100)), 2);
        var itemTotal = Math.Round(discountedUnitPrice * item.Quantity, 2);

        return new OrderPreviewItemDTO
        {
            ProductId = item.ProductId,
            ProductName = product.Name,
            Quantity = item.Quantity,
            UnitPrice = product.Price,
            DiscountPercentage = discountPct,
            DiscountedUnitPrice = discountedUnitPrice,
            ItemTotal = itemTotal,
        };
    }

    private static double CalculateSubtotal(List<(OrderItem Item, Product Product)> itemsWithProducts)
    {
        return itemsWithProducts.Sum(i => i.Product.Price * i.Item.Quantity);
    }

    private static double CalculateTotalDiscount(List<(OrderItem Item, Product Product)> itemsWithProducts, List<Promotion> promotions)
    {
        double discount = 0;

        foreach (var (item, product) in itemsWithProducts)
        {
            var discountPct = GetDiscountPercentage(item.ProductId, promotions);
            if (discountPct > 0)
            {
                discount += (product.Price * item.Quantity) * (discountPct / 100);
            }
        }

        return Math.Round(discount, 2);
    }

    private static (double Vat, double Total) CalculateTotals(double subtotal, double discount, double shippingCost)
    {
        var discountedSubtotal = subtotal - discount;
        var vat = Math.Round(discountedSubtotal * AppConstants.Vat, 2);
        var total = Math.Round((discountedSubtotal * (1 + AppConstants.Vat)) + shippingCost, 2);
        return (vat, total);
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
}
