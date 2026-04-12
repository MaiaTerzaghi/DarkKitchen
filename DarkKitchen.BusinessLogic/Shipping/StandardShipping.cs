using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Shipping;

public class StandardShipping : IShippingStrategy
{
    public double CalculateCost() => 20.0;
}
