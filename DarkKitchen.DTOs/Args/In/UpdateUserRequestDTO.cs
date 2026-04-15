using DarkKitchen.Domain.Enums;

namespace DarkKitchen.DTOs.Args.In;

public class UpdateUserRequestDTO
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
}
