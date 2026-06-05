using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Importers.Contracts;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class ImportServiceTest
{
    private Mock<IImporterProvider> _providerMock = null!;
    private Mock<IProductService> _productServiceMock = null!;
    private Mock<IImageFileReader> _imageReaderMock = null!;
    private ImportService _service = null!;
    private const string ImagesRoot = "/test-images";

    [TestInitialize]
    public void Setup()
    {
        _providerMock = new Mock<IImporterProvider>();
        _productServiceMock = new Mock<IProductService>();
        _imageReaderMock = new Mock<IImageFileReader>();
        _service = new ImportService(
            _providerMock.Object,
            _productServiceMock.Object,
            _imageReaderMock.Object,
            ImagesRoot);
    }

    [TestMethod]
    public void GetAvailableImporters_WhenProviderHasImporters_ReturnsMappedDTOs()
    {
        _providerMock.Setup(p => p.GetImporterNames())
                     .Returns(["JSON", "XML"]);

        var result = _service.GetAvailableImporters();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(i => i.Name == "JSON"));
        Assert.IsTrue(result.Any(i => i.Name == "XML"));
    }

    [TestMethod]
    public void Import_WhenImporterReturnsOneProduct_CallsCreateProductAndCountsIt()
    {
        var request = new ImportProductsRequestDTO
        {
            ImporterName = "JSON",
            Content = "[{...}]",
            FileName = "products.json"
        };

        var importedProduct = new ImportedProduct
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Una rica pizza napolitana con tomate",
            Price = 250.0,
            CommercialLine = "Minutas",
            Category = "Pizzas"
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>()))
                    .Returns([importedProduct]);

        _providerMock.Setup(p => p.GetByName("JSON"))
                    .Returns(importerMock.Object);

        var result = _service.Import(request, "admin@email.com");

        Assert.AreEqual(1, result.ImportedCount);

        _productServiceMock.Verify(s => s.CreateProduct(
            It.Is<CreateProductRequestDTO>(dto =>
                dto.Code == "P0001" &&
                dto.Name == "Pizza Napolitana" &&
                dto.Description == "Una rica pizza napolitana con tomate" &&
                dto.Price == 250.0 &&
                dto.CommercialLine == "Minutas" &&
                dto.Category == "Pizzas"),
            "admin@email.com"), Times.Once);
    }

    [TestMethod]
    public void Import_WhenProductHasImagePaths_ConvertsThemToBase64JoinedByComma()
    {
        var imageBytes1 = new byte[] { 1, 2, 3 };
        var imageBytes2 = new byte[] { 4, 5, 6 };
        var expectedBase64 = $"{Convert.ToBase64String(imageBytes1)},{Convert.ToBase64String(imageBytes2)}";

        var imported = new ImportedProduct
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Una rica pizza napolitana con tomate",
            Price = 250.0,
            CommercialLine = "Minutas",
            Category = "Pizzas",
            ImagePaths = ["pizza1.jpg", "pizza2.jpg"]
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([imported]);
        _providerMock.Setup(p => p.GetByName("JSON")).Returns(importerMock.Object);

        _imageReaderMock.Setup(r => r.Read(Path.Combine(ImagesRoot, "pizza1.jpg"))).Returns(imageBytes1);
        _imageReaderMock.Setup(r => r.Read(Path.Combine(ImagesRoot, "pizza2.jpg"))).Returns(imageBytes2);

        var request = new ImportProductsRequestDTO { ImporterName = "JSON", Content = "x", FileName = "x.json" };

        var result = _service.Import(request, "admin@email.com");

        Assert.AreEqual(1, result.ImportedCount);
        _productServiceMock.Verify(s => s.CreateProduct(
            It.Is<CreateProductRequestDTO>(dto => dto.Images == expectedBase64),
            "admin@email.com"), Times.Once);
    }

    [TestMethod]
    public void Import_WhenRowFailsValidation_AddsToErrorsAndDoesNotCallCreateProduct()
    {
        var invalid = new ImportedProduct
        {
            Code = " ",
            Name = "Pizza Napolitana",
            Description = "Una rica pizza napolitana con tomate",
            Price = 250.0,
            CommercialLine = "Minutas",
            Category = "Pizzas"
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([invalid]);
        _providerMock.Setup(p => p.GetByName("JSON")).Returns(importerMock.Object);

        var request = new ImportProductsRequestDTO
        {
            ImporterName = "JSON",
            Content = "x",
            FileName = "x.json"
        };

        var result = _service.Import(request, "admin@email.com");

        Assert.AreEqual(0, result.ImportedCount);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(0, result.Errors[0].Index);
        Assert.AreEqual(" ", result.Errors[0].Code);

        _productServiceMock.Verify(
            s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>(), It.IsAny<string>()),
            Times.Never);
    }
}
