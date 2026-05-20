using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.States;

public sealed class DelayedState : OrderStateBase
{
    protected override string DisplayName => "Demorado";

    public override void Prepare(Order order) => order.Status = OrderStatus.Prepared;
    public override void Cancel(Order order) => order.Status = OrderStatus.Cancelled;
}
