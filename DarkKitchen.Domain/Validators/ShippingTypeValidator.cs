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

    public static void ValidateCost(double cost)
    {
        if (cost <= 0)
        {
            throw new ArgumentException("El costo debe ser mayor a cero.");
        }
    }
}
