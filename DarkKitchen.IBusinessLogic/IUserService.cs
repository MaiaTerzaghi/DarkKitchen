using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IUserService
{
    int Register(RegisterClientDTO request);
    int CreateStaffUser(CreateStaffUserRequestDTO request);
    PaginatedResponse<UserResponseDTO> GetUsers(string? name, string? lastName, int page = 1, int pageSize = 20);
    UserResponseDTO UpdateUser(int id, UpdateUserRequestDTO request, int requestingUserId);
    void DeleteUser(int id, int requestingUserId);
}
