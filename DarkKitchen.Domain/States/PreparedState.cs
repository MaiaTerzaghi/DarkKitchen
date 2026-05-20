using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.States;

public sealed class PreparedState : OrderStateBase
{
    protected override string DisplayName => "Preparado";

    public override void MarkOnTheWay(Order order) => order.Status = OrderStatus.OnTheWay;
}
