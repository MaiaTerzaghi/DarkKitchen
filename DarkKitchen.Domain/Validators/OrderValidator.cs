namespace DarkKitchen.Domain.Validators;

public static class OrderValidator
{
    public static void ValidateStreet(string street)
    {
        if(string.IsNullOrWhiteSpace(street))
        {
            throw new ArgumentException("La calle no puede estar vacía.");
        }
    }

    public static void ValidateDoorNumber(string doorNumber)
    {
        if(string.IsNullOrWhiteSpace(doorNumber))
        {
            throw new ArgumentException("El número de puerta no puede estar vacío.");
        }
    }
}
