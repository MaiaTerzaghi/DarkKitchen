using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class ShippingTypeServiceTest
{
    private Mock<IRepository<ShippingType>> _repositoryMock = null!;
    private ShippingTypeService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IRepository<ShippingType>>();
        _service = new ShippingTypeService(_repositoryMock.Object);
    }

    [TestMethod]
    public void GetAll_WhenCalled_ReturnsAllShippingTypes()
    {
        var shippingTypes = new List<ShippingType>
        {
            new ShippingType { Id = 1, Name = "Envío express", Cost = 250 },
            new ShippingType { Id = 2, Name = "Envío en el día", Cost = 200 }
        };

        _repositoryMock.Setup(r => r.GetAll(null, null, false, 1, 20))
                       .Returns(shippingTypes);

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Envío express", result[0].Name);
        Assert.AreEqual(250, result[0].Cost);
    }

    [TestMethod]
    public void GetById_WhenExists_ReturnsShippingType()
    {
        var shippingType = new ShippingType { Id = 1, Name = "Envío express", Cost = 250 };

        _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<ShippingType, bool>>>()))
                       .Returns(shippingType);

        var result = _service.GetById(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Envío express", result.Name);
        Assert.AreEqual(250, result.Cost);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void GetById_WhenNotExists_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<ShippingType, bool>>>()))
                       .Returns((ShippingType?)null);

        _service.GetById(999);
    }

    [TestMethod]
    public void Create_WhenValidData_ReturnsShippingTypeResponse()
    {
        var request = new CreateShippingTypeRequestDTO
        {
            Name = "Envío express",
            Cost = 250
        };

        var saved = new ShippingType { Id = 1, Name = "Envío express", Cost = 250 };

        _repositoryMock.Setup(r => r.Add(It.IsAny<ShippingType>()))
                       .Returns(saved);

        var result = _service.Create(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Envío express", result.Name);
        Assert.AreEqual(250, result.Cost);
    }
}
