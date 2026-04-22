using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

internal sealed class DbContextBuilder
{
    private readonly static SqliteConnection _connection = new("Data Source=:memory:");

    public static TestDbContext BuildTestDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _connection.Open();
        var context = new TestDbContext(options);
        return context;
    }
}
