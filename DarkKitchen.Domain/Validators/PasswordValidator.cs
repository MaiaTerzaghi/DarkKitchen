using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Validators;

public static class PasswordValidator
{
    private const int MinLength = 15;
    private const int MaxLength = 25;
    private const string UppercasePattern = @"[A-Z]";
    private const string LowercasePattern = @"[a-z]";
    private const string NumberPattern = @"[0-9]";
    private const string SymbolPattern = @"[^a-zA-Z0-9]";
    private const string SequencePattern = @"012|123|234|345|456|567|678|789|987|876|765|654|543|432|321|210";
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
        if(password.Length < MinLength || password.Length > MaxLength)
        {
            throw new ArgumentException("La contraseña debe tener entre 15 y 25 caracteres");
        }
    }

    private static void ValidateUppercase(string password)
    {
        if(!Regex.IsMatch(password, UppercasePattern))
        {
            throw new ArgumentException("La contraseña debe tener al menos una mayúscula");
        }
    }

    private static void ValidateLowercase(string password)
    {
        if(!Regex.IsMatch(password, LowercasePattern))
        {
            throw new ArgumentException("La contraseña debe tener al menos una minúscula");
        }
    }

    private static void ValidateNumber(string password)
    {
        if(!Regex.IsMatch(password, NumberPattern))
        {
            throw new ArgumentException("La contraseña debe tener al menos un número");
        }
    }

    private static void ValidateSymbol(string password)
    {
        if(!Regex.IsMatch(password, SymbolPattern))
        {
            throw new ArgumentException("La contraseña debe tener al menos un símbolo");
        }
    }

    private static void ValidateNoSequence(string password)
    {
        if(Regex.IsMatch(password, SequencePattern))
        {
            throw new ArgumentException("La contraseña no debe tener secuencias de números");
        }
    }
}
