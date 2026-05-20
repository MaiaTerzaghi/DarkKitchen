using DarkKitchen.ServiceFactory;
using DarkKitchen.WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Filters.Add<ExceptionFilter>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

DarkKitchen.ServiceFactory.ServiceRegistration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

SeedData.SeedAdminUser(app.Services);

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
