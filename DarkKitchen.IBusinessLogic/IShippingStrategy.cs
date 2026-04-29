using DarkKitchen.Domain.Enums;
namespace DarkKitchen.IBusinessLogic;

public interface IShippingStrategy
{
    DeliveryType Type { get; }
    double CalculateCost();
}
