using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IUserService
{
    string Login(string email, string password);
    int Register(User user);
}
