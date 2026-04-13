using DarkKitchen.Domain.Entities;
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
        throw new NotImplementedException();
    }
}
