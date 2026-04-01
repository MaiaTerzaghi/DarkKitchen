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

    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
