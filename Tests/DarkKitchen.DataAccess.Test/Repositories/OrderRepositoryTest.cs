using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class OrderRepositoryTest
{
    private SqliteConnection? _connection;
    private DarkKitchenContext? _context;

    [TestInitialize]
    public void Initialize()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
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
    public void Save_ValidOrder_ReturnsSavedOrderWithId()
    {
        var order = new Order
        {
            ClientId = 1,
            DeliveryType = "Express",
            Status = "Pending",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "2B",
            Items = []
        };

        var repository = new OrderRepository(_context!);
        var result = repository.Save(order);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
    }

    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ReturnsOrders()
    {
        var order = new Order
        {
            ClientId = 1,
            DeliveryType = "Express",
            Status = "Pending",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var request = new GetClientOrdersRequestDTO { ClientId = 1 };

        var result = repository.GetClientOrders(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].ClientId);
    }
}
