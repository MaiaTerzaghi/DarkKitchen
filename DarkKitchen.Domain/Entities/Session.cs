// using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Entities;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid(); // propiedad id para EF

    public string Token { get; set; } = Guid.NewGuid().ToString();

    public int UserId { get; set; }
    public User? User { get; set; }
}
