using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Validators;

public static class UserValidator
{
    private const int MinLastNameLength = 3;
    private const int MaxLastNameLength = 25;
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+.[^@\s]+$";
    private const string PhonePattern = @"^+\d{7,15}$";

    public static void ValidateEmail(string email)
    {
        if(!Regex.IsMatch(email, EmailPattern))
        {
            throw new ArgumentException("El email no tiene un formato válido");
        }
    }

    public static void ValidateName(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("El nombre no puede estar vacío");
        }
    }

    public static void ValidateLastName(string lastName)
    {
        if(lastName.Length < MinLastNameLength || lastName.Length > MaxLastNameLength)
        {
            throw new ArgumentException(
                $"El apellido debe tener entre {MinLastNameLength} y {MaxLastNameLength} caracteres");
        }
    }

    public static void ValidatePhone(string phone)
    {
        if(!Regex.IsMatch(phone, PhonePattern))
        {
            throw new ArgumentException("El teléfono no tiene un formato válido");
        }
    }
}
