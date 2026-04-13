namespace DarkKitchen.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    private string _code = string.Empty;
    public string Code
    {
        get => _code;
        set
        {
            if (value.Length < 5)
            {
                throw new ArgumentException("El código debe tener minimo 5.");
            }

            _code = value;
        }
    }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public string CommercialLine { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Images { get; set; } = string.Empty;
}
