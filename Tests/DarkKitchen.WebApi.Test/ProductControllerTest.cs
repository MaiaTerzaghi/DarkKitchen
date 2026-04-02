using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
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
            new Product { Id = 1, Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Images = "im1.jpg" },
            new Product { Id = 2, Name = "Pasta", Category = "Pastas", CommercialLine = "Minutas",  Images = "im2.jpg" },
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
}
