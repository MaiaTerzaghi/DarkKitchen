namespace DarkKitchen.DTOs.Args.Output;

public class OrderPreviewResponseDTO
{
    public List<OrderPreviewItemDTO> Items { get; set; } = [];
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double Vat { get; set; }
    public double ShippingCost { get; set; }
    public double Total { get; set; }
}

public class OrderPreviewItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPercentage { get; set; }
    public double DiscountedUnitPrice { get; set; }
    public double ItemTotal { get; set; }
}
