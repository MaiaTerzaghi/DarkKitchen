namespace DarkKitchen.BusinessLogic.Args.In;

public class CreateOrderRequestDTO
{
    public Guid ClientId { get; set; }
    public string DeliveryType { get; set; } = string.Empty;
    public AddressDTO Address { get; set; } = null!;
    public List<OrderItemRequestDTO> Items { get; set; } = [];
}
