namespace DarkKitchen.Domain.Entities;

public class Promotion
{
    private string _name = string.Empty;
    private decimal _discountPercentage;
    private DateTime _validFrom;
    private DateTime _validTo;

    public int Id { get; set; }
    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre de la promoción no puede estar vacío.");
            }

            _name = value;
        }
    }

    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            if(value <= 0 || value > 100)
            {
                throw new ArgumentException("El porcentaje de descuento debe ser mayor que 0 y menor o igual a 100.");
            }

            _discountPercentage = value;
        }
    }

    public DateTime ValidFrom
    {
        get => _validFrom;
        set => _validFrom = value;
    }

    public DateTime ValidTo
    {
        get => _validTo;
        set
        {
            if(value < _validFrom)
            {
                throw new ArgumentException("La fecha de fin no puede ser menor que la fecha de inicio.");
            }

            _validTo = value;
        }
    }

    public string ProductLine { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
}
