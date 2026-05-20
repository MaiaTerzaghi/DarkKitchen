namespace DarkKitchen.Domain.States;

public sealed class NotDeliveredState : OrderStateBase
{
    protected override string DisplayName => "No entregado";
}
