namespace DarkKitchen.Domain.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"El email {email} ya está registrado.")
    {
    }
}
