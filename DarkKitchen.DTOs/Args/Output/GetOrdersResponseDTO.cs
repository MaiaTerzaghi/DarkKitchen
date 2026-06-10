namespace DarkKitchen.DTOs.Args.Output;

public class GetOrdersResponseDTO
{
    public int OrderId { get; set; }
    public ClientInfoDTO Client { get; set; } = new();
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public double Total { get; set; }
    public int ItemCount { get; set; }
    public List<OrderItemResponseDTO> Items { get; set; } = [];
}
