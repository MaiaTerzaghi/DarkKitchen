using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Entities;

public class User
{
    public int Id { get; set; }
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _name = string.Empty;
    private string _lastName = string.Empty;
    private string _phone = string.Empty;
    public UserRole Role { get; set; }
    public string Email
    {
        get => _email;
        set
        {
            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception("El email no tiene un formato válido");
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
                throw new Exception("El nombre no puede estar vacío");
            }

            _name = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if(value.Length < 3 || value.Length > 25)
            {
                throw new Exception("El apellido debe tener entre 3 y 25 caracteres");
            }

            _lastName = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\+\d{7,15}$"))
            {
                throw new Exception("El teléfono no tiene un formato válido");
            }

            _phone = value;
        }
    }
}
