using DarkKitchen.Domain.Enums;
namespace DarkKitchen.Domain.Entities;

public class Order
{
    private string _street = string.Empty;
    private string _doorNumber = string.Empty;
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
    public string DoorNumber
    {
        get => _doorNumber;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El número de puerta no puede estar vacío.");
            }

            _doorNumber = value;
        }
    }

    public string? Apartment { get; set; }
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double ShippingCost { get; set; }
    public double Vat { get; set; }
    public double Total { get; set; }
    public DateTime UpdatedAt { get; set; }
}
