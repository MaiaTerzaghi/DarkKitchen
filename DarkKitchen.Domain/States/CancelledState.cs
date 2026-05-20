namespace DarkKitchen.Domain.States;

public sealed class CancelledState : OrderStateBase
{
    protected override string DisplayName => "Cancelado";
}
