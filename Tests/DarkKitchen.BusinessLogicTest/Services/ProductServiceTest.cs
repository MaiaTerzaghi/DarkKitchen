using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class ProductServiceTest
{
    [TestMethod]
    public void GetAll_WhenNoFilters_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Pizza" },
            new Product { Id = 2, Name = "Pasta" },
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.GetAll(null, null, null))
                             .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object);

        var result = productService.GetAll(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_WhenFilterByName_ReturnsFilteredProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Pizza" },
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.GetAll("Pizza", null, null))
                             .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object);

        var result = productService.GetAll("Pizza", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza", result[0].Name);
    }

    [TestMethod]
    public void CreateProduct_WhenValidData_ReturnsProductResponse()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var product = new Product
        {
            Id = 1,
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object);
        var result = productService.CreateProduct(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("P0001", result.Code);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCodeIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                     .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCodeIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "CODIGO123456789012345",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNameIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request);
    }
}
