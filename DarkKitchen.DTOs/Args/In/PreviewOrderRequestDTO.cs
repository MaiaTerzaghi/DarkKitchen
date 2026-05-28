namespace DarkKitchen.DTOs.Args.In;

public class PreviewOrderRequestDTO
{
    public string ShippingType { get; set; } = string.Empty;
    public List<OrderItemRequestDTO> Items { get; set; } = [];
}
