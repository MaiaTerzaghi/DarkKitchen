namespace DarkKitchen.DTOs.Args.Output;

public class GetOrdersResponseDTO
{
    public int OrderId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponseDTO> Items { get; set; } = [];
}
