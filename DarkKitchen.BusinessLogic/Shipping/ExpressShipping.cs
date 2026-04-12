using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class ExpressShipping : IShippingStrategy
{
    public double CalculateCost() => 50.0;
}
