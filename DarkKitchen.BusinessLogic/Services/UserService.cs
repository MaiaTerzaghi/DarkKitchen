using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

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
}
