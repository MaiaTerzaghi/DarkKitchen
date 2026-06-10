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
    public DbSet<ShippingType> ShippingTypes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingType)
            .WithMany()
            .HasForeignKey(o => o.ShippingTypeId)
            .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Client)
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .Property(a => a.EntityName)
            .HasConversion<string>();
    }
}
