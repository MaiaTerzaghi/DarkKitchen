using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.IBusinessLogic;

public class PricingResult
{
    public List<OrderItem> Items { get; set; } = [];
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double ShippingCost { get; set; }
    public double Vat { get; set; }
    public double Total { get; set; }
}

public interface IPricingService
{
    PricingResult CalculateOrderPricing(List<OrderItemRequestDTO> items, DeliveryType deliveryType);
}
