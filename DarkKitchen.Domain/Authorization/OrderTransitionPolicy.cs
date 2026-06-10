using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Authorization;

public static class OrderTransitionPolicy
{
    private static readonly Dictionary<OrderStatus, UserRole[]> AllowedRolesByTarget = new()
    {
        { OrderStatus.Prepared, [UserRole.Administrative, UserRole.Dispatcher] },
        { OrderStatus.Cancelled, [UserRole.Administrative] },
        { OrderStatus.OnTheWay, [UserRole.Dispatcher] },
        { OrderStatus.Delivered, [UserRole.Dispatcher] },
        { OrderStatus.NotDelivered, [UserRole.Dispatcher] },
        { OrderStatus.Delayed, [UserRole.Dispatcher] },
    };

    public static void AssertCanTransition(UserRole role, OrderStatus target)
    {
        if(!AllowedRolesByTarget.TryGetValue(target, out var allowed) || !allowed.Contains(role))
        {
            throw new UnauthorizedException(
                $"El rol {role} no puede cambiar el estado del pedido a {target}.");
        }
    }
}
