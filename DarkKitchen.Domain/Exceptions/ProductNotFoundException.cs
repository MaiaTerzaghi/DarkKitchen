namespace DarkKitchen.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int id)
        : base($"No se encontró un producto con id {id}.")
    {
    }
}
