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
            UserValidator.ValidateEmail(value);
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set => _password = value;
    }

    public string Name
    {
        get => _name;
        set
        {
            UserValidator.ValidateName(value);
            _name = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            UserValidator.ValidateLastName(value);
            _lastName = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            UserValidator.ValidatePhone(value);
            _phone = value;
        }
    }
}
