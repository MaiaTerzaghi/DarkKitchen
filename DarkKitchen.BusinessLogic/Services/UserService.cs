using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
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
}
