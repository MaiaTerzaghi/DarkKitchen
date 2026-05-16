using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
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
}
