using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Context;

public sealed class DarkKitchenContext(DbContextOptions<DarkKitchenContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Product> Products { get; set; }
}
