namespace DarkKitchen.WebApi.DTOs;

public class RegisterClientDTO
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Password { get; set; }
}
