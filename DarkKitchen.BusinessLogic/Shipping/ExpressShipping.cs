using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class ExpressShipping : IShippingStrategy
{
    private const double Cost = 50.0;
    public double CalculateCost() => Cost;
}
