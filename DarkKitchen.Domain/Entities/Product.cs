namespace DarkKitchen.Domain.Entities;

public class Product
{
    private const int MinCodeLength = 5;
    private const int MaxCodeLength = 20;
    private const int MinNameLength = 10;
    private const int MaxNameLength = 50;
    private const int MinDescriptionLength = 20;
    private const int MaxDescriptionLength = 500;
    private const int MaxImages = 3;
    private const string ImageExtension = ".jpg";
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _commercialLine = string.Empty;
    private string _category = string.Empty;
    private double _price;
    private string _images = string.Empty;
    public int Id { get; set; }
    public string Code
    {
        get => _code;
        set
        {
            if(value.Length < MinCodeLength || value.Length > MaxCodeLength)
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
            if(value.Length < MinNameLength || value.Length > MaxNameLength)
            {
                throw new ArgumentException("El nombre debe tener entre 10 y 50 caracteres.");
            }

            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if(value.Length < MinDescriptionLength || value.Length > MaxDescriptionLength)
            {
                throw new ArgumentException("La descripción debe tener entre 20 y 500 caracteres.");
            }

            _description = value;
        }
    }

    public double Price
    {
        get => _price;
        set
        {
            if(value < 0)
            {
                throw new ArgumentException("El precio debe ser mayor a cero.");
            }

            _price = value;
        }
    }

    public string CommercialLine
    {
        get => _commercialLine;
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("La línea comercial no puede estar vacía.");
            }

            _commercialLine = value;
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("La categoría no puede estar vacía.");
            }

            _category = value;
        }
    }

    public bool IsActive { get; set; } = true;
    public string Images
    {
        get => _images;
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Se requiere al menos una imagen.");
            }

            var images = value.Split(',');

            if(images.Length > MaxImages)
            {
                throw new ArgumentException("Se permiten hasta 3 imágenes.");
            }

            if(images.Any(img => !img.Trim().EndsWith(ImageExtension)))
            {
                throw new ArgumentException("Las imágenes deben ser en formato jpg.");
            }

            _images = value;
        }
    }
}
