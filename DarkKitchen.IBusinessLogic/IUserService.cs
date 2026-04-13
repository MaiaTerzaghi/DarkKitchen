using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.IBusinessLogic;
public interface IUserService
{
    int Register(User user);
    int CreateStaffUser(CreateStaffUserRequestDTO request);
}
