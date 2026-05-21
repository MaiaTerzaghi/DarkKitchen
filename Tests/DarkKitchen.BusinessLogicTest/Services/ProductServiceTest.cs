using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class ProductServiceTest
{
    private Mock<IRepository<Product>> _productRepositoryMock = null!;
    private ProductService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IRepository<Product>>();
        _service = new ProductService(_productRepositoryMock.Object);
    }

    [TestMethod]
    public void GetAll_WhenNoFilters_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Pizza Napolitana" },
            new Product { Id = 2, Name = "Pasta Bolognese" },
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
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
            new Product { Id = 1, Name = "Pizza Napolitana" },
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                             .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object);

        var result = productService.GetAll("Pizza Napolitana", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
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

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object);
        var result = productService.CreateProduct(request, "admin@email.com");

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

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                     .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
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

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
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

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNameIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana con tomate fresco y albahaca italiana muy larga",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenDescriptionIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenDescriptionIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = new string('a', 501),
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCommercialLineIsEmpty_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = string.Empty,
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCategoryIsEmpty_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = string.Empty,
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenPriceIsNegative_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = -1.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNoImages_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = string.Empty
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenMoreThanThreeImages_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza1.jpg,pizza2.jpg,pizza3.jpg,pizza4.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImageIsNotJpg_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.png"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = "P0001", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate", CommercialLine = "Minutas", Category = "Fritos", Images = "pizza.jpg" });

        var productService = new ProductService(productRepositoryMock.Object);
        productService.CreateProduct(request, "admin@email.com");
    }

    [TestMethod]
    public void UpdateProduct_WhenProductExists_ReturnsUpdatedProduct()
    {
        var request = new UpdateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
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
            Images = "pizza.jpg",
            IsActive = true
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                            .Returns(product);
        productRepositoryMock.Setup(r => r.Update(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object);
        var result = productService.UpdateProduct(1, request);

        Assert.IsNotNull(result);
        Assert.AreEqual("P0001", result.Code);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void UpdateProduct_WhenProductNotFound_ThrowsException()
    {
        var request = new UpdateProductRequestDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                            .Returns((Product?)null);

        var productService = new ProductService(productRepositoryMock.Object);
        productService.UpdateProduct(999, request);
    }

    [TestMethod]
    public void GetManage_WhenNoFilters_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Code = "P0001",
                Name = "Pizza Napolitana",
                Description = "Rica pizza napolitana con tomate y albahaca",
                Price = 100.0,
                CommercialLine = "Minutas",
                Category = "Fritos",
                Images = "pizza.jpg",
                IsActive = true
            }
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                            .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object);
        var result = productService.GetManage(new GetProductsManageRequestDTO());

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }
}
