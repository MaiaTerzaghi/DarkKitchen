using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class ServiceRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DarkKitchen");
        if(string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Missing DarkKitchen connection string");
        }

        services.AddDbContext<DarkKitchenContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<DbContext, DarkKitchenContext>();
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Product>, Repository<Product>>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IOrderService, OrderService>();
    }
}
