namespace DarkKitchen.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public Guid ClientId { get; set; }
    public string DeliveryType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = [];
    public string Street { get; set; } = string.Empty;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
}
