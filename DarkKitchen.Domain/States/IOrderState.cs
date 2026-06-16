using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.States;

public interface IOrderState
{
    void Prepare(Order order);
    void Cancel(Order order);
    void MarkOnTheWay(Order order);
    void Deliver(Order order);
    void MarkNotDelivered(Order order);
    void MarkDelayed(Order order);
}
