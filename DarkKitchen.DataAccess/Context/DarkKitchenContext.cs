using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Context;

public sealed class DarkKitchenContext(DbContextOptions<DarkKitchenContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Guardo el tipo de delivery como string en la bdd
        modelBuilder.Entity<Order>()
            .Property(o => o.DeliveryType)
            .HasConversion<string>();

        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Promotion>()
            .HasMany(p => p.Products)
            .WithMany()
            .UsingEntity(j => j.ToTable("PromotionProducts"));

        modelBuilder.Entity<Promotion>()
            .Property(p => p.DiscountPercentage)
            .HasPrecision(18, 2);
    }
}
