using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces;

public interface IUserRepository
{
    User GetByEmail(string email);
}
