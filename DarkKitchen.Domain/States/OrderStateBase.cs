using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.States;

public abstract class OrderStateBase : IOrderState
{
    protected abstract string DisplayName { get; }

    public virtual void Prepare(Order order) =>
        throw new ConflictException($"No se puede preparar un pedido en estado {DisplayName}.");

    public virtual void Cancel(Order order) =>
        throw new ConflictException($"No se puede cancelar un pedido en estado {DisplayName}.");

    public virtual void MarkOnTheWay(Order order) =>
        throw new ConflictException($"No se puede poner en camino un pedido en estado {DisplayName}.");

    public virtual void Deliver(Order order) =>
        throw new ConflictException($"No se puede entregar un pedido en estado {DisplayName}.");

    public virtual void MarkNotDelivered(Order order) =>
        throw new ConflictException($"No se puede marcar como no entregado un pedido en estado {DisplayName}.");

    public virtual void MarkDelayed(Order order) =>
        throw new ConflictException($"No se puede marcar como demorado un pedido en estado {DisplayName}.");
}
