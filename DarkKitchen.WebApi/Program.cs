using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Interfaces;

// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
