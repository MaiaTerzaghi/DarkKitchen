using DarkKitchen.ServiceFactory;
using DarkKitchen.WebApi.Filters;

namespace DarkKitchen.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
            options.Filters.Add<GlobalExceptionFilterAttribute>());

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        ServiceRegistration.RegisterServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        SeedData.SeedAdminUser(app.Services);

        app.UseCors("AllowAngular");

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
