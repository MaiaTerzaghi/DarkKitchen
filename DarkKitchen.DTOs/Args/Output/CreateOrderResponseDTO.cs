namespace DarkKitchen.DTOs.Args.Output;

public class CreateOrderResponseDTO
{
    public int ClientId { get; set; }
    public int OrderId { get; set; }
    public double Subtotal { get; set; }
    public double Vat { get; set; }
    public double ShippingCost { get; set; }
    public double Total { get; set; }
}
