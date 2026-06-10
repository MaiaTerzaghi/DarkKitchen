using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Auditing;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
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
    private Mock<IAuditSubject> _auditSubjectMock = null!;

    private const string P0001 = "P0001";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string Fritos = "Fritos";
    private const string Minutas = "Minutas";
    private const string RicaPizzaNapolitanaConTomateYA = "Rica pizza napolitana con tomate y albahaca";
    private const string AdminEmailCom = "admin@email.com";
    private const string RicaPizzaNapolitanaConTomate = "Rica pizza napolitana con tomate";
    private const string ValidJpgBase64 = "/9j/2Q==";

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IRepository<Product>>();
        _auditSubjectMock = new Mock<IAuditSubject>();
        _service = new ProductService(_productRepositoryMock.Object, _auditSubjectMock.Object);
    }

    [TestMethod]
    public void GetAll_WhenNoFilters_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = PizzaNapolitana },
            new Product { Id = 2, Name = "Pasta Bolognese" },
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                             .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);

        var result = productService.GetAll(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_WhenFilterByName_ReturnsFilteredProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = PizzaNapolitana },
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                             .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);

        var result = productService.GetAll(PizzaNapolitana, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(PizzaNapolitana, result[0].Name);
    }

    [TestMethod]
    public void CreateProduct_WhenValidData_ReturnsProductResponse()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var product = new Product
        {
            Id = 1,
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        var result = productService.CreateProduct(request, AdminEmailCom);

        Assert.IsNotNull(result);
        Assert.AreEqual(P0001, result.Code);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCodeIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "P001",
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                     .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCodeIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = "CODIGO123456789012345",
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNameIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = "Pizza",
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNameIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = "Pizza Napolitana con tomate fresco y albahaca italiana muy larga",
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenDescriptionIsTooShort_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = "Rica pizza",
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenDescriptionIsTooLong_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = new string('a', 501),
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCommercialLineIsEmpty_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = string.Empty,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenCategoryIsEmpty_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = string.Empty,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenPriceIsNegative_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = -1.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenNoImages_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = string.Empty
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenMoreThanThreeImages_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = "pizza1.jpg,pizza2.jpg,pizza3.jpg,pizza4.jpg"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImageIsNotJpg_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = "pizza.png"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                            .Returns(new Product { Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    public void UpdateProduct_WhenProductExists_ReturnsUpdatedProduct()
    {
        var request = new UpdateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64,
            IsActive = true
        };

        var product = new Product
        {
            Id = 1,
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64,
            IsActive = true
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                            .Returns(product);
        productRepositoryMock.Setup(r => r.Update(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        var result = productService.UpdateProduct(1, request, AdminEmailCom);

        Assert.IsNotNull(result);
        Assert.AreEqual(P0001, result.Code);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void UpdateProduct_WhenProductNotFound_ThrowsException()
    {
        var request = new UpdateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64,
            IsActive = true
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                            .Returns((Product?)null);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.UpdateProduct(999, request, AdminEmailCom);
    }

    [TestMethod]
    public void GetManage_WhenNoFilters_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Code = P0001,
                Name = PizzaNapolitana,
                Description = RicaPizzaNapolitanaConTomateYA,
                Price = 100.0,
                CommercialLine = Minutas,
                Category = Fritos,
                Images = ValidJpgBase64,
                IsActive = true
            }
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                            .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        var result = productService.GetManage(new GetProductsManageRequestDTO());

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetManage_MapsIdDescriptionAndActiveStatus()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 42,
                Code = P0001,
                Name = PizzaNapolitana,
                Description = RicaPizzaNapolitanaConTomateYA,
                Price = 100.0,
                CommercialLine = Minutas,
                Category = Fritos,
                Images = ValidJpgBase64,
                IsActive = false
            }
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), null, false, 1, 20))
                            .Returns(products);

        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        var result = productService.GetManage(new GetProductsManageRequestDTO());

        Assert.AreEqual(42, result[0].Id);
        Assert.AreEqual(RicaPizzaNapolitanaConTomateYA, result[0].Description);
        Assert.IsFalse(result[0].IsActive);
    }

    [TestMethod]
    public void CreateProduct_WhenValidData_NotifiesAuditSubject()
    {
        var auditSubjectMock = new Mock<IAuditSubject>();
        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
            .Returns(new Product { Id = 5, Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, auditSubjectMock.Object);
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        productService.CreateProduct(request, AdminEmailCom);

        auditSubjectMock.Verify(a => a.Notify(It.Is<AuditEvent>(e =>
            e.EntityName == AuditedEntity.Product &&
            e.EntityId == 5 &&
            e.ResponsibleUser == AdminEmailCom)), Times.Once);
    }

    [TestMethod]
    public void CreateProduct_WhenValidData_NotifiesAuditWithDescription()
    {
        var auditSubjectMock = new Mock<IAuditSubject>();
        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
            .Returns(new Product { Id = 5, Code = P0001, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomate, CommercialLine = Minutas, Category = Fritos, Images = ValidJpgBase64 });

        var productService = new ProductService(productRepositoryMock.Object, auditSubjectMock.Object);
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64
        };

        productService.CreateProduct(request, AdminEmailCom);

        auditSubjectMock.Verify(a => a.Notify(It.Is<AuditEvent>(e =>
            e.Description.Contains(P0001))), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_WhenProductExists_NotifiesAuditSubject()
    {
        var auditSubjectMock = new Mock<IAuditSubject>();
        var product = new Product
        {
            Id = 3,
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64,
            IsActive = true
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        productRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                            .Returns(product);
        productRepositoryMock.Setup(r => r.Update(It.IsAny<Product>()))
                            .Returns(product);

        var productService = new ProductService(productRepositoryMock.Object, auditSubjectMock.Object);
        var request = new UpdateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ValidJpgBase64,
            IsActive = true
        };

        productService.UpdateProduct(3, request, AdminEmailCom);

        auditSubjectMock.Verify(a => a.Notify(It.Is<AuditEvent>(e =>
            e.EntityName == AuditedEntity.Product &&
            e.EntityId == 3 &&
            e.ResponsibleUser == AdminEmailCom &&
            e.Description.Contains(P0001))), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImagesAreOnlyCommas_ThrowsException()
    {
        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = ",,"
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImageTooLarge_ThrowsException()
    {
        var bigBytes = new byte[(500 * 1024) + 1];
        bigBytes[0] = 0xFF;
        bigBytes[1] = 0xD8;
        bigBytes[2] = 0xFF;
        var bigImage = Convert.ToBase64String(bigBytes);

        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = bigImage
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImageIsNotJpeg_ThrowsException()
    {
        var notJpeg = Convert.ToBase64String(new byte[] { 0x01, 0x02, 0x03 });

        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = notJpeg
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WhenImageTooShortToBeJpeg_ThrowsException()
    {
        var tooShort = Convert.ToBase64String(new byte[] { 0xFF, 0xD8 });

        var request = new CreateProductRequestDTO
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateYA,
            Price = 100.0,
            CommercialLine = Minutas,
            Category = Fritos,
            Images = tooShort
        };

        var productRepositoryMock = new Mock<IRepository<Product>>();
        var productService = new ProductService(productRepositoryMock.Object, _auditSubjectMock.Object);
        productService.CreateProduct(request, AdminEmailCom);
    }
}
