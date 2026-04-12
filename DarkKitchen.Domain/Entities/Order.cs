using DarkKitchen.Domain.Enums;
namespace DarkKitchen.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public string Street { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public DateTime UpdatedAt { get; set; }
}
