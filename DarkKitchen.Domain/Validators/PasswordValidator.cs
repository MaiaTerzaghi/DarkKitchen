using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Validators;

public static class PasswordValidator
{
    public static void Validate(string password)
    {
        ValidateLength(password);
        ValidateUppercase(password);
        ValidateLowercase(password);
        ValidateNumber(password);
        ValidateSymbol(password);
        ValidateNoSequence(password);
    }

    private static void ValidateLength(string password)
    {
        if(password.Length < 15 || password.Length > 25)
        {
            throw new Exception("La contraseña debe tener entre 15 y 25 caracteres");
        }
    }

    private static void ValidateUppercase(string password)
    {
        if(!Regex.IsMatch(password, @"[A-Z]"))
        {
            throw new Exception("La contraseña debe tener al menos una mayúscula");
        }
    }

    private static void ValidateLowercase(string password)
    {
        if(!Regex.IsMatch(password, @"[a-z]"))
        {
            throw new Exception("La contraseña debe tener al menos una minúscula");
        }
    }

    private static void ValidateNumber(string password)
    {
        if(!Regex.IsMatch(password, @"[0-9]"))
        {
            throw new Exception("La contraseña debe tener al menos un número");
        }
    }

    private static void ValidateSymbol(string password)
    {
        if(!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
        {
            throw new Exception("La contraseña debe tener al menos un símbolo");
        }
    }

    private static void ValidateNoSequence(string password)
    {
        if(Regex.IsMatch(password, @"012|123|234|345|456|567|678|789"))
        {
            throw new Exception("La contraseña no debe tener secuencias de números");
        }
    }
}
