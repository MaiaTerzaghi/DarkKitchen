using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Validators;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IRepository<User> userRepository, IPasswordManager passwordManager) : IUserService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public int Register(RegisterClientDTO request)
    {
        var existingUser = _userRepository.Get(u => u.Email == request.Email);
        if(existingUser != null)
        {
            throw new ConflictException("El mail ya esta registrado");
        }

        PasswordValidator.Validate(request.Password);

        var user = new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = _passwordManager.ComputeHash(request.Password),
            Role = UserRole.Client
        };

        var savedUser = _userRepository.Add(user);
        return savedUser.Id;
    }

    public int CreateStaffUser(CreateStaffUserRequestDTO request)
    {
        var existingUser = _userRepository.Get(u => u.Email == request.Email);
        if(existingUser != null)
        {
            throw new ConflictException("El mail ya está registrado");
        }

        if(request.Role != UserRole.Administrative && request.Role != UserRole.Dispatcher)
        {
            throw new ArgumentException("El rol debe ser Administrativo o Preparador");
        }

        PasswordValidator.Validate(request.Password);

        var user = new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = request.Password,
            Role = request.Role
        };

        var savedUser = _userRepository.Add(user);
        return savedUser.Id;
    }

    public List<UserResponseDTO> GetUsers(string? name, string? lastName)
    {
        var users = _userRepository.GetAll(
            predicate: u =>
            (name == null || u.Name.Contains(name)) &&
            (lastName == null || u.LastName.Contains(lastName)));

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
        if(id == requestingUserId)
        {
            throw new ArgumentException("Un usuario no puede modificarse a sí mismo");
        }

        var user = _userRepository.Get(u => u.Id == id) ?? throw new NotFoundException("Usuario no encontrado");

        PasswordValidator.Validate(request.Password);

        user!.Name = request.Name;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Password = request.Password;
        user.Role = request.Role;

        var updated = _userRepository.Update(user);

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

    public void DeleteUser(int id, int requestingUserId)
    {
        if(id == requestingUserId)
        {
            throw new ArgumentException("Un usuario no puede eliminarse a sí mismo");
        }

        var user = _userRepository.Get(u => u.Id == id) ?? throw new NotFoundException("Usuario no encontrado");

        _userRepository.Delete(user);
    }
}
