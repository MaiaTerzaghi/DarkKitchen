namespace DarkKitchen.Domain.Validators;

public static class PromotionValidator
{
    private const decimal MinDiscountPercentage = 0;
    private const decimal MaxDiscountPercentage = 100;

    public static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la promoción no puede estar vacío.");
        }
    }

    public static void ValidateDiscountPercentage(decimal discountPercentage)
    {
        if(discountPercentage <= MinDiscountPercentage || discountPercentage > MaxDiscountPercentage)
        {
            throw new ArgumentException(
                $"El porcentaje de descuento debe ser mayor que {MinDiscountPercentage} y menor o igual a {MaxDiscountPercentage}.");
        }
    }

    public static void ValidateDateRange(DateTime validFrom, DateTime validTo)
    {
        if(validTo < validFrom)
        {
            throw new ArgumentException("La fecha de fin no puede ser menor que la fecha de inicio.");
        }
    }
}
