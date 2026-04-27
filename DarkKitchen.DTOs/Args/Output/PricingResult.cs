using DarkKitchen.Domain.Entities;

namespace DarkKitchen.DTOs.Args.Output;

public class PricingResult
{
    public List<OrderItem> Items { get; set; } = [];
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double ShippingCost { get; set; }
    public double Vat { get; set; }
    public double Total { get; set; }
}
