using DarkKitchen.Domain.Enums;

namespace DarkKitchen.DTOs.Args.Output;

public class UserResponseDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public UserRole Role { get; set; }
}
