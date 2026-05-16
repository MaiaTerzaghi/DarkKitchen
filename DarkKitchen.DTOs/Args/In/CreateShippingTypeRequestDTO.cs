namespace DarkKitchen.DTOs.Args.In;

public class CreateShippingTypeRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public double Cost { get; set; }
}
