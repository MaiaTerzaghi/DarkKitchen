namespace DarkKitchen.Domain.Exceptions;

public class UserNotFoundException(string email)
    : Exception($"Usuario con email {email} no encontrado.")
{
}
