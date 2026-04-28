using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class StandardShipping : IShippingStrategy
{
    private const double Cost = 20.0;
    public DeliveryType Type => DeliveryType.Standard;
    public double CalculateCost() => Cost;
}
