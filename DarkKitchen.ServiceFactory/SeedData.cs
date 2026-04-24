using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class SeedData
{
    public static void SeedAdminUser(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var passwordManager = scope.ServiceProvider.GetRequiredService<IPasswordManager>();

        var adminEmail = "admin@email.com";
        var adminPassword = "Administrador1#$%";

        var existing = userRepository.Get(u => u.Email == adminEmail);
        if(existing != null)
        {
            return;
        }

        var admin = new User
        {
            Name = "Admin",
            LastName = "Principal",
            Email = adminEmail,
            Phone = "+59899000000",
            Password = passwordManager.ComputeHash(adminPassword),
            Role = UserRole.Administrative
        };

        userRepository.Add(admin);
    }
}
