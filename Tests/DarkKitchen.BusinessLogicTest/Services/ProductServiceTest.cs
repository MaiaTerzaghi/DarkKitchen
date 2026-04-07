using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
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
}
