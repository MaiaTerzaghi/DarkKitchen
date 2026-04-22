using DarkKitchen.Domain.Enums;
namespace DarkKitchen.Domain.Entities;

public class Order
{
    private string _street = string.Empty;
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public string Street
    {
        get => _street;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("La calle no puede estar vacía.");
            }

            _street = value;
        }
    }

    public DateTime Date { get; set; } = DateTime.Now;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public DateTime UpdatedAt { get; set; }
}
