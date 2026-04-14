using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public int Register(User user)
    {
        var existingUser = _userRepository.GetByEmail(user.Email);
        if(existingUser != null)
        {
            throw new ArgumentException("El mail ya esta registrado");
        }

        return _userRepository.AddUser(user);
    }

    public int CreateStaffUser(CreateStaffUserRequestDTO request)
    {
        var existingUser = _userRepository.GetByEmail(request.Email);
        if(existingUser != null)
        {
            throw new ArgumentException("El mail ya está registrado");
        }

        if(request.Role != UserRole.Administrative && request.Role != UserRole.Dispatcher)
        {
            throw new ArgumentException("El rol debe ser Administrativo o Preparador");
        }

        var user = new User
            {
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password,
                Role = request.Role
            };

        return _userRepository.AddUser(user);
    }

    public List<UserResponseDTO> GetUsers(string? name, string? lastName)
    {
        var users = _userRepository.GetUsers(name, lastName);

        return users.Select(u => new UserResponseDTO
        {
            Id = u.Id,
            Name = u.Name,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role
        }).ToList();
    }

    public UserResponseDTO UpdateUser(int id, UpdateUserRequestDTO request, int requestingUserId)
    {
        var user = _userRepository.GetById(id) ?? throw new ArgumentException("Usuario no encontrado");

        user!.Name = request.Name;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Password = request.Password;
        user.Role = request.Role;

        var updated = _userRepository.UpdateUser(user);

        return new UserResponseDTO
        {
            Id = updated.Id,
            Name = updated.Name,
            LastName = updated.LastName,
            Email = updated.Email,
            Phone = updated.Phone,
            Role = updated.Role
        };
        }
}
