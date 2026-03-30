// using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Entities;

public class Session
{
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public User? User { get; set; }
}
