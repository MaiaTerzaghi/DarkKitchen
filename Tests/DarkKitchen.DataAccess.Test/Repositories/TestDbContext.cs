using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

internal sealed class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<EntityTest> EntitiesTest { get; set; }
}

internal sealed record class EntityTest()
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;

    public EntityTest(string name)
        : this()
    {
        Name = name;
    }
}
