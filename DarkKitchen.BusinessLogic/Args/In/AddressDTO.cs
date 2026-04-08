namespace DarkKitchen.BusinessLogic.Args.In;

public class AddressDTO
{
    public string Street { get; set; } = string.Empty;
    public string DoorNumber { get; set; } = string.Empty;
    public string? Apartment { get; set; }
}
