namespace DarkKitchen.DTOs.Args.Output;

public class OrderDetailResponseDTO
{
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShippingType { get; set; } = string.Empty;
    public double Total { get; set; }
    public List<OrderItemDetailDTO> Items { get; set; } = [];
}
