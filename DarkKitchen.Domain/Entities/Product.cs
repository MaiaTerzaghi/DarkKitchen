namespace DarkKitchen.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    private string _code = string.Empty;
    private string _name = string.Empty;
    public string Code
    {
        get => _code;
        set
        {
            if (value.Length < 5 || value.Length > 20)
            {
                throw new ArgumentException("El código debe tener entre 5 y 20 caracteres.");
            }

            _code = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value.Length < 10 || value.Length > 50)
            {
                throw new ArgumentException("El nombre debe tener entre 10 y 50 caracteres.");
            }

            _name = value;
        }
    }

    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public string CommercialLine { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Images { get; set; } = string.Empty;
}
