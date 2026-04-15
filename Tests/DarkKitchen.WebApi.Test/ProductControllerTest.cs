using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class ProductControllerTest
{
    [TestMethod]
    public void GetAll_WhenNoFilters_ReturnsOk()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Pizza Napolitana", Category = "Fritos", CommercialLine = "Minutas", Images = "im1.jpg" },
            new Product { Id = 2, Name = "Pasta bolognese", Category = "Pastas", CommercialLine = "Minutas",  Images = "im2.jpg" },
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetAll(null, null, null))
                          .Returns(products);

        var controller = new ProductController(productServiceMock.Object);

        var result = controller.GetAll(null, null, null);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.IsNotNull(okResult.Value);
    }

    [TestMethod]
    public void ProductResponseDTO_WhenCreated_HasCorrectProperties()
    {
        var dto = new ProductResponseDTO
        {
            Code = "P001",
            Name = "Pizza Napolitana",
            Price = 100,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "img1.jpg"
        };

        Assert.AreEqual("P001", dto.Code);
        Assert.AreEqual("Pizza Napolitana", dto.Name);
        Assert.AreEqual(100, dto.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAll_WhenServiceThrowsException_ReturnsBadRequest()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetAll(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                        .Throws(new ArgumentException("Error"));

        var controller = new ProductController(productServiceMock.Object);

        controller.GetAll(null, null, null);
    }

    [TestMethod]
    public void CreateProduct_WhenCalled_ReturnsCreated()
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

        var response = new ProductResponseDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>()))
                        .Returns(response);

        var controller = new ProductController(productServiceMock.Object);
        var result = controller.CreateProduct(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void UpdateProduct_WhenCalled_ReturnsOk()
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

        var response = new ProductResponseDTO
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<UpdateProductRequestDTO>()))
                        .Returns(response);

        var controller = new ProductController(productServiceMock.Object);
        var result = controller.UpdateProduct(1, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetManage_WhenCalled_ReturnsOk()
    {
        var products = new List<ProductResponseDTO>
        {
            new ProductResponseDTO
            {
                Code = "P0001",
                Name = "Pizza Napolitana",
                Price = 100.0,
                CommercialLine = "Minutas",
                Category = "Fritos",
                Images = "pizza.jpg"
            }
        };

        var request = new GetProductsManageRequestDTO();

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetManage(It.IsAny<GetProductsManageRequestDTO>()))
                        .Returns(products);

        var controller = new ProductController(productServiceMock.Object);
        var result = controller.GetManage(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
}
