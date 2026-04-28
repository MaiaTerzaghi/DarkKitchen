namespace DarkKitchen.Domain.Validators;

public static class ProductValidator
{
    private const int MinCodeLength = 5;
    private const int MaxCodeLength = 20;
    private const int MinNameLength = 10;
    private const int MaxNameLength = 50;
    private const int MinDescriptionLength = 20;
    private const int MaxDescriptionLength = 500;
    private const int MaxImages = 3;
    private const string ImageExtension = ".jpg";

    public static void ValidateCode(string code)
    {
        if(code.Length < MinCodeLength || code.Length > MaxCodeLength)
        {
            throw new ArgumentException(
                $"El código debe tener entre {MinCodeLength} y {MaxCodeLength} caracteres.");
        }
    }

    public static void ValidateName(string name)
    {
        if(name.Length < MinNameLength || name.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"El nombre debe tener entre {MinNameLength} y {MaxNameLength} caracteres.");
        }
    }

    public static void ValidateDescription(string description)
    {
        if(description.Length < MinDescriptionLength || description.Length > MaxDescriptionLength)
        {
            throw new ArgumentException(
                $"La descripción debe tener entre {MinDescriptionLength} y {MaxDescriptionLength} caracteres.");
        }
    }

    public static void ValidatePrice(double price)
    {
        if(price <= 0)
        {
            throw new ArgumentException("El precio debe ser mayor a cero.");
        }
    }

    public static void ValidateCommercialLine(string commercialLine)
    {
        if(string.IsNullOrEmpty(commercialLine))
        {
            throw new ArgumentException("La línea comercial no puede estar vacía.");
        }
    }

    public static void ValidateCategory(string category)
    {
        if(string.IsNullOrEmpty(category))
        {
            throw new ArgumentException("La categoría no puede estar vacía.");
        }
    }

    public static void ValidateImages(string images)
    {
        if(string.IsNullOrEmpty(images))
        {
            throw new ArgumentException("Se requiere al menos una imagen.");
        }

        var imageList = images.Split(',');

        if(imageList.Length > MaxImages)
        {
            throw new ArgumentException($"Se permiten hasta {MaxImages} imágenes.");
        }

        if(imageList.Any(img => !img.Trim().EndsWith(ImageExtension)))
        {
            throw new ArgumentException($"Las imágenes deben ser en formato {ImageExtension}.");
        }
    }
}
