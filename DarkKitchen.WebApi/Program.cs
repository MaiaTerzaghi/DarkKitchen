using DarkKitchen.WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Filters.Add<ExceptionFilter>());

DarkKitchen.ServiceFactory.ServiceRegistration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
