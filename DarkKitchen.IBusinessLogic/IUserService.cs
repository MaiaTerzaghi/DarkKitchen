using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IUserService
{
    int Register(User user);
    int CreateStaffUser(CreateStaffUserRequestDTO request);
    List<UserResponseDTO> GetUsers(string? name, string? lastName);
    UserResponseDTO UpdateUser(int id, UpdateUserRequestDTO request);
}
