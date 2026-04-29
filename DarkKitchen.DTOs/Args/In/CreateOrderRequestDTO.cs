namespace DarkKitchen.DTOs.Args.In;

public class CreateOrderRequestDTO
{
    public int ClientId { get; set; }
    public string DeliveryType { get; set; } = string.Empty;
    public AddressDTO Address { get; set; } = null!;
    public List<OrderItemRequestDTO> Items { get; set; } = [];
}
