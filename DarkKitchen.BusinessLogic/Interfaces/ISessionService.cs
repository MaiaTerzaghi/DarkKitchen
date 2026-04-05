using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface ISessionService
{
    User GetUserFromToken(string token);
}
