using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class ExpressShipping : IShippingStrategy
{
    private const double Cost = 50.0;
    public DeliveryType Type => DeliveryType.Express;
    public double CalculateCost() => Cost;
}
