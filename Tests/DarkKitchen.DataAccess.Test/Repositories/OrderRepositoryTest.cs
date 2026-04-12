using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
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
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
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
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
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

    [TestMethod]
    public void GetOrders_FilterByDateRange_ReturnsOrdersWithinRange()
    {
        var order1 = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 3, 10),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);

        var request = new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 1, 31)
        };

        var result = repository.GetOrders(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(order1.Id, result[0].Id);
    }

    [TestMethod]
    public void GetOrders_FilterByStreet_ReturnsOrdersMatchingStreet()
    {
        var order1 = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 1, 15),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);

        var request = new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 1, 31),
            Street = "18 de Julio"
        };

        var result = repository.GetOrders(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("18 de Julio", result[0].Street);
    }

    [TestMethod]
    public void GetOrders_FilterByStatus_ReturnsOrdersMatchingStatus()
    {
        var order1 = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Delivered,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 1, 15),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);

        var request = new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 1, 31),
            Status = OrderStatus.Pending
        };

        var result = repository.GetOrders(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(OrderStatus.Pending, result[0].Status);
    }

    [TestMethod]
    public void GetById_ExistingOrder_ReturnsOrder()
    {
        var order = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetById(order.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
        Assert.AreEqual(OrderStatus.Pending, result.Status);
    }

    [TestMethod]
    public void Update_ExistingOrder_ReturnsUpdatedOrder()
    {
        var order = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        order.Status = OrderStatus.Prepared;

        var repository = new OrderRepository(_context);
        var result = repository.Update(order);

        Assert.IsNotNull(result);
        Assert.AreEqual(OrderStatus.Prepared, result.Status);
    }

    [TestMethod]
    public void GetOrderById_WhenOrderExists_ReturnsOrder()
    {
        var order = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = "Pending",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetOrderById(order.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
    }
}
