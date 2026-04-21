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
        var result = repository.GetOrderById(order.Id);

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
    public void GetTopProducts_ValidRequest_ReturnsProductsInDateRange()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "pizza.jpg"
        };

        var orderInRange = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = [new OrderItem { Product = product, Quantity = 5 }]
        };

        var orderOutOfRange = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 3, 10),
            Items = [new OrderItem { Product = product, Quantity = 10 }]
        };

        _context!.Orders.AddRange(orderInRange, orderOutOfRange);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Product.Name);
        Assert.AreEqual(5, result[0].Quantity);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsProductsOrderedByQuantity()
    {
        var product1 = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "pizza.jpg"
        };

        var product2 = new Product
        {
            Code = "P0002",
            Name = "Pasta Bolognesa",
            Description = "Rica pasta bolognesa",
            CommercialLine = "Minutas",
            Category = "Pastas",
            Price = 80.0,
            Images = "pasta.jpg"
        };

        var order = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items =
            [
                new OrderItem { Product = product1, Quantity = 5 },
                new OrderItem { Product = product2, Quantity = 10 }
            ]
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.AreEqual("Pasta Bolognesa", result[0].Product.Name);
        Assert.AreEqual("Pizza Napolitana", result[1].Product.Name);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsLimitedResults()
    {
        for(var i = 1; i <= 6; i++)
        {
            var product = new Product
            {
                Code = $"P000{i}",
                Name = $"Producto {i} largo nombre",
                Description = "Descripcion larga del producto",
                CommercialLine = "Minutas",
                Category = "Fritos",
                Price = 100.0,
                Images = "img.jpg"
            };

            var order = new Order
            {
                ClientId = 1,
                DeliveryType = DeliveryType.Express,
                Status = OrderStatus.Delivered,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Date = new DateTime(2026, 1, 10),
                Items = [new OrderItem { Product = product, Quantity = i }]
            };

            _context!.Orders.Add(order);
        }

        _context!.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsSalesGroupedByYearMonthAndClient()
    {
        var order1 = new Order
        {
            ClientId = 1,
            DeliveryType = DeliveryType.Express,
            Status = OrderStatus.Delivered,
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
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 15),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetSalesReport(1, 20);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2026, result[0].Year);
        Assert.AreEqual(1, result[0].Month);
        Assert.AreEqual(1, result[0].ClientId);
    }

    [TestMethod]
    public void GetSalesReport_WithPagination_ReturnsCorrectPage()
    {
        for(var i = 1; i <= 25; i++)
        {
            _context!.Orders.Add(new Order
            {
                ClientId = i,
                DeliveryType = DeliveryType.Express,
                Status = OrderStatus.Delivered,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Date = new DateTime(2026, i % 12 + 1, 1),
                Items = []
            });
        }

        _context!.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetSalesReport(2, 20);

        Assert.AreEqual(5, result.Count);
    }
}
