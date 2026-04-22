using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class StandardShipping : IShippingStrategy
{
    private const double Cost = 20.0;
    public double CalculateCost() => Cost;
}
