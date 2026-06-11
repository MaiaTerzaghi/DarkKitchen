using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class ProductControllerTest
{
    private const string Minutas = "Minutas";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string Requestinguser = "RequestingUser";
    private const string Fritos = "Fritos";
    private const string AdminEmailCom = "admin@email.com";
    private const string P0001 = "P0001";
    private const string PizzaJpg = "pizza.jpg";
    private const string P001 = "P001";
    private const string RicaPizzaNapolitanaConTomateYA = "Rica pizza napolitana con tomate y albahaca";
    private const string Error = "Error";
    private const string PastaBolognese = "Pasta bolognese";
    private const string Pastas = "Pastas";
    private const string Im1Jpg = "im1.jpg";
    private const string Im2Jpg = "im2.jpg";
    private const string Img1Jpg = "img1.jpg";

    [TestMethod]
    public void GetProducts_WhenNoFilters_ReturnsOk()
    {
        var paginatedResponse = new PaginatedResponse<ProductResponseDTO>
        {
            Items =
            [
                new ProductResponseDTO { Name = PizzaNapolitana, Category = Fritos, CommercialLine = Minutas, Images = Im1Jpg },
                new ProductResponseDTO { Name = PastaBolognese, Category = Pastas, CommercialLine = Minutas, Images = Im2Jpg },
            ],
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetProducts(It.IsAny<GetProductsManageRequestDTO>(), It.IsAny<UserRole>()))
                          .Returns(paginatedResponse);

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Administrative };

        var result = controller.GetProducts(new GetProductsManageRequestDTO());

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetProducts_WhenClientRole_PassesClientRole()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetProducts(It.IsAny<GetProductsManageRequestDTO>(), It.IsAny<UserRole>()))
                          .Returns(new PaginatedResponse<ProductResponseDTO>());

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Client };

        var request = new GetProductsManageRequestDTO();
        controller.GetProducts(request);

        productServiceMock.Verify(s => s.GetProducts(request, UserRole.Client), Times.Once);
    }

    [TestMethod]
    public void ProductResponseDTO_WhenCreated_HasCorrectProperties()
    {
        var dto = new ProductResponseDTO
        {
            Code = P001,
            Name = PizzaNapolitana,
            Price = 100,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = Img1Jpg
        };

        Assert.AreEqual(P001, dto.Code);
        Assert.AreEqual(PizzaNapolitana, dto.Name);
        Assert.AreEqual(100, dto.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetProducts_WhenServiceThrowsException_Throws()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetProducts(It.IsAny<GetProductsManageRequestDTO>(), It.IsAny<UserRole>()))
                        .Throws(new ArgumentException(Error));

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Administrative };

        controller.GetProducts(new GetProductsManageRequestDTO());
    }

    [TestMethod]
    public void CreateProduct_WhenCalled_ReturnsCreated()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = PizzaJpg
        };

        var response = new ProductResponseDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = PizzaJpg
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>(), It.IsAny<string>()))
                        .Returns(response);

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Email = AdminEmailCom };

        var result = controller.CreateProduct(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }

    [TestMethod]
    public void UpdateProduct_WhenCalled_ReturnsOk()
    {
        var request = new UpdateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = PizzaJpg,
            IsActive = true
        };

        var response = new ProductResponseDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = PizzaJpg
        };

        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<UpdateProductRequestDTO>(), It.IsAny<string>()))
                .Returns(response);

        var controller = new ProductController(productServiceMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Email = AdminEmailCom };
        var result = controller.UpdateProduct(1, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void CreateProduct_PassesResponsibleUserFromContextToService()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>(), It.IsAny<string>()))
            .Returns(new ProductResponseDTO());

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Email = AdminEmailCom };

        var request = new CreateProductRequestDTO();
        controller.CreateProduct(request);

        productServiceMock.Verify(s => s.CreateProduct(request, AdminEmailCom), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_PassesResponsibleUserFromContextToService()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<UpdateProductRequestDTO>(), It.IsAny<string>()))
            .Returns(new ProductResponseDTO());

        var controller = new ProductController(productServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Email = AdminEmailCom };

        var request = new UpdateProductRequestDTO();
        controller.UpdateProduct(7, request);

        productServiceMock.Verify(s => s.UpdateProduct(7, request, AdminEmailCom), Times.Once);
    }
}
