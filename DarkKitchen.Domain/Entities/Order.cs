using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Entities;

public class Order
{
    private string _street = string.Empty;
    private string _doorNumber = string.Empty;
    public int Id { get; set; }
    public int ClientId { get; set; }
    public User Client { get; set; } = null!;
    public int ShippingTypeId { get; set; }
    public ShippingType ShippingType { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public string? Apartment { get; set; }
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double ShippingCost { get; set; }
    public double Vat { get; set; }
    public double Total { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Street
    {
        get => _street;
        set
        {
            OrderValidator.ValidateStreet(value);
            _street = value;
        }
    }

    public DateTime Date { get; set; } = DateTime.Now;
    public string DoorNumber
    {
        get => _doorNumber;
        set
        {
            OrderValidator.ValidateDoorNumber(value);
            _doorNumber = value;
        }
    }
}
