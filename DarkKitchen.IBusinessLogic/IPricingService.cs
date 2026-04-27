using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;

public interface IPricingService
{
    PricingResult CalculateOrderPricing(List<OrderItemRequestDTO> items, DeliveryType deliveryType);
}
