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

    public int CreateUser(CreateUserRequestDTO request)
    {
        ValidateEmailNotTaken(request.Email);
        PasswordValidator.Validate(request.Password);

        var role = ResolveRole(request.Role);
        var user = BuildUser(request, role);
        var savedUser = _userRepository.Add(user);
        return savedUser.Id;
    }

    private static UserRole ResolveRole(UserRole? role)
    {
        if(role is null)
        {
            return UserRole.Client;
        }

        if(role is not(UserRole.Administrative or UserRole.Dispatcher))
        {
            throw new ArgumentException("El rol debe ser Administrativo o Preparador");
        }

        return role.Value;
    }

    private void ValidateEmailNotTaken(string email)
    {
        var existingUser = _userRepository.Get(u => u.Email == email);
        if(existingUser != null)
        {
            throw new ConflictException("El mail ya está registrado");
        }
    }

    private User BuildUser(CreateUserRequestDTO request, UserRole role)
    {
        return new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = _passwordManager.ComputeHash(request.Password),
            Role = role
        };
    }

    public PaginatedResponse<UserResponseDTO> GetUsers(string? name, string? lastName, int page = 1, int pageSize = 20)
    {
        var (users, totalCount) = _userRepository.GetAll(
            predicate: u =>
            (name == null || u.Name.Contains(name)) &&
            (lastName == null || u.LastName.Contains(lastName)),
            page: page,
            pageSize: pageSize);

        return new PaginatedResponse<UserResponseDTO>
        {
            Items = users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Name = u.Name,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public UserResponseDTO UpdateUser(int id, UpdateUserRequestDTO request, int requestingUserId)
    {
        if(id == requestingUserId)
        {
            throw new ArgumentException("Un usuario no puede modificarse a sí mismo");
        }

        var user = _userRepository.Get(u => u.Id == id) ?? throw new NotFoundException("Usuario no encontrado");

        user!.Name = request.Name;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        if(!string.IsNullOrEmpty(request.Password))
        {
            PasswordValidator.Validate(request.Password);
            user.Password = _passwordManager.ComputeHash(request.Password);
        }

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
