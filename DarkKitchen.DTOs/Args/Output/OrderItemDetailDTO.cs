namespace DarkKitchen.DTOs.Args.Output;

public class OrderItemDetailDTO
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Subtotal { get; set; }
}
