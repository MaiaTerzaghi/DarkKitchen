using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.States;

public sealed class PendingState : OrderStateBase
{
    protected override string DisplayName => "Pendiente";

    public override void Prepare(Order order) => order.Status = OrderStatus.Prepared;
    public override void Cancel(Order order) => order.Status = OrderStatus.Cancelled;
    public override void MarkDelayed(Order order) => order.Status = OrderStatus.Delayed;
}
