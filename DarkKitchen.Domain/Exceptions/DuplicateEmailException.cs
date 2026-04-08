namespace DarkKitchen.Domain.Exceptions;

public class DuplicateEmailException(string email)
    : Exception($"El email {email} ya está registrado.")
{
}
