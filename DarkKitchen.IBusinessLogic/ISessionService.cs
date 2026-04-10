using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IBusinessLogic;
public interface ISessionService
{
    User GetUserFromToken(string token);
}
