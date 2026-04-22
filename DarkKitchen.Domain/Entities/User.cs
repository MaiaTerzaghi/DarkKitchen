using System.Text.RegularExpressions;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Entities;

public class User
{
    private const int MinLastNameLength = 3;
    private const int MaxLastNameLength = 25;
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private const string PhonePattern = @"^\+\d{7,15}$";
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _name = string.Empty;
    private string _lastName = string.Empty;
    private string _phone = string.Empty;
    public int Id { get; set; }
    public UserRole Role { get; set; }
    public string Email
    {
        get => _email;
        set
        {
            if(!Regex.IsMatch(value, EmailPattern))
            {
                throw new ArgumentException("El email no tiene un formato válido");
            }

            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            PasswordValidator.Validate(value);
            _password = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("El nombre no puede estar vacío");
            }

            _name = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if(value.Length < MinLastNameLength || value.Length > MaxLastNameLength)
            {
                throw new ArgumentException("El apellido debe tener entre 3 y 25 caracteres");
            }

            _lastName = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if(!Regex.IsMatch(value, PhonePattern))
            {
                throw new ArgumentException("El teléfono no tiene un formato válido");
            }

            _phone = value;
        }
    }
}
