using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
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
            if (value.Length < 3 || value.Length > 25)
            {
                throw new Exception("El apellido debe tener entre 3 y 25 caracteres");
            }

            _lastName = value;
        }
    }

    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
