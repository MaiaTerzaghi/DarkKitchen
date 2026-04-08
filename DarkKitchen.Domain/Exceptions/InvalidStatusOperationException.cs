namespace DarkKitchen.Domain.Exceptions;

public class InvalidOrderStatusException(string currentStatus)
    : Exception($"Operación inválida. El pedido se encuentra en estado: {currentStatus}.")
{
}
