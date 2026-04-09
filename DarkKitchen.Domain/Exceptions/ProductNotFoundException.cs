namespace DarkKitchen.Domain.Exceptions;

public class ProductNotFoundException(int id)
    : Exception($"Producto con id {id} no encontrado.")
{
}
