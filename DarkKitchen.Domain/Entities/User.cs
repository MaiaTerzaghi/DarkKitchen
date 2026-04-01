using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class User
{
    public int Id { get; set; }
    private string _email = string.Empty;
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

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            if(value.Length < 15 || value.Length > 25)
            {
                throw new Exception("La contraseña debe tener entre 15 y 25 caracteres");
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"[A-Z]"))
            {
                throw new Exception("La contraseña debe tener al menos una mayúscula");
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"[a-z]"))
            {
                throw new Exception("La contraseña debe tener al menos una minúscula");
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"[0-9]"))
            {
                throw new Exception("La contraseña debe tener al menos un número");
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"[^a-zA-Z0-9]"))
            {
                throw new Exception("La contraseña debe tener al menos un símbolo");
            }

            if(System.Text.RegularExpressions.Regex.IsMatch(value, @"012|123|234|345|456|567|678|789"))
            {
                throw new Exception("La contraseña no debe tener secuencias de números");
            }

            _password = value;
        }
    }

    private string _name = string.Empty;
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

    private string _lastName = string.Empty;
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

    private string _phone = string.Empty;
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

    public UserRole Role { get; set; }
}
