namespace DarkKitchen.Domain.Validators;

public static class ShippingTypeValidator
{
    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del tipo de envío no puede estar vacío.");
        }
    }
}
