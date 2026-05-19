using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.States;

public sealed class OnTheWayState : OrderStateBase
{
    protected override string DisplayName => "En camino";

    public override void Deliver(Order order) => order.Status = OrderStatus.Delivered;
    public override void MarkNotDelivered(Order order) => order.Status = OrderStatus.NotDelivered;
}
