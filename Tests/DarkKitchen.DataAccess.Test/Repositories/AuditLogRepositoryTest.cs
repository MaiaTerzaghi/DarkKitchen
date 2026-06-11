using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class AuditLogRepositoryTest
{
    private SqliteConnection? _connection;

    private const string AdminEmailCom = "admin@email.com";
    private const string AltaDeProducto = "Alta de producto.";
    private const string AltaDePromocion = "Alta de promoción.";
    private const string DataSourceMemory = "Data Source=:memory:";
    private DarkKitchenContext? _context;

    [TestInitialize]
    public void Initialize()
    {
        _connection = new SqliteConnection(DataSourceMemory);
        _connection.Open();

        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
        _connection?.Dispose();
    }

    [TestMethod]
    public void GetByEntity_WhenMatchesEntityAndRange_ReturnsLog()
    {
        _context!.AuditLogs.Add(new AuditLog
        {
            Timestamp = new DateTime(2026, 4, 23, 9, 0, 0),
            EntityName = AuditedEntity.Product,
            EntityId = 12345,
            Description = AltaDeProducto,
            ResponsibleUser = AdminEmailCom
        });
        _context.SaveChanges();

        var repository = new AuditLogRepository(_context);
        var result = repository.GetByEntity(
            AuditedEntity.Product,
            12345,
            new DateTime(2026, 4, 23, 8, 0, 0),
            new DateTime(2026, 4, 23, 10, 0, 0));

        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void GetByEntity_WhenTimestampOutOfRange_ReturnsEmpty()
    {
        _context!.AuditLogs.Add(new AuditLog
        {
            Timestamp = new DateTime(2026, 4, 23, 12, 0, 0),
            EntityName = AuditedEntity.Product,
            EntityId = 12345,
            Description = AltaDeProducto,
            ResponsibleUser = AdminEmailCom
        });
        _context.SaveChanges();

        var repository = new AuditLogRepository(_context);
        var result = repository.GetByEntity(
            AuditedEntity.Product,
            12345,
            new DateTime(2026, 4, 23, 8, 0, 0),
            new DateTime(2026, 4, 23, 10, 0, 0));

        Assert.AreEqual(0, result.Items.Count);
    }

    [TestMethod]
    public void GetByEntity_WhenEntityIdDoesNotMatch_ReturnsEmpty()
    {
        _context!.AuditLogs.Add(new AuditLog
        {
            Timestamp = new DateTime(2026, 4, 23, 9, 0, 0),
            EntityName = AuditedEntity.Product,
            EntityId = 999,
            Description = AltaDeProducto,
            ResponsibleUser = AdminEmailCom
        });
        _context.SaveChanges();

        var repository = new AuditLogRepository(_context);
        var result = repository.GetByEntity(
            AuditedEntity.Product,
            12345,
            new DateTime(2026, 4, 23, 8, 0, 0),
            new DateTime(2026, 4, 23, 10, 0, 0));

        Assert.AreEqual(0, result.Items.Count);
    }

    [TestMethod]
    public void GetByEntity_WhenEntityNameDoesNotMatch_ReturnsEmpty()
    {
        _context!.AuditLogs.Add(new AuditLog
        {
            Timestamp = new DateTime(2026, 4, 23, 9, 0, 0),
            EntityName = AuditedEntity.Promotion,
            EntityId = 12345,
            Description = AltaDePromocion,
            ResponsibleUser = AdminEmailCom
        });
        _context.SaveChanges();

        var repository = new AuditLogRepository(_context);
        var result = repository.GetByEntity(
            AuditedEntity.Product,
            12345,
            new DateTime(2026, 4, 23, 8, 0, 0),
            new DateTime(2026, 4, 23, 10, 0, 0));

        Assert.AreEqual(0, result.Items.Count);
    }
}
