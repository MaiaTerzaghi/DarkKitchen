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

    private const string Json = "JSON";
    private const string AdminEmailCom = "admin@email.com";
    private const string Minutas = "Minutas";
    private const string P0001 = "P0001";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string Pizzas = "Pizzas";
    private const string UnaRicaPizzaNapolitanaConTomat = "Una rica pizza napolitana con tomate";
    private const string PizzaJpg = "pizza.jpg";
    private const string Pizza1Jpg = "pizza1.jpg";
    private const string Pizza2Jpg = "pizza2.jpg";
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
                     .Returns([Json, "XML"]);

        var result = _service.GetAvailableImporters();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(i => i.Name == Json));
        Assert.IsTrue(result.Any(i => i.Name == "XML"));
    }

    [TestMethod]
    public void Import_WhenImporterReturnsOneProduct_CallsCreateProductAndCountsIt()
    {
        var request = new ImportProductsRequestDTO
        {
            ImporterName = Json,
            Content = "[{...}]",
            FileName = "products.json"
        };

        var imageBytes = new byte[] { 1, 2, 3 };

        var importedProduct = new ImportedProduct
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = UnaRicaPizzaNapolitanaConTomat,
            Price = 250.0,
            CommercialLine = Minutas,
            Category = Pizzas,
            ImagePaths = [PizzaJpg]
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>()))
                    .Returns([importedProduct]);

        _providerMock.Setup(p => p.GetByName(Json))
                    .Returns(importerMock.Object);

        _imageReaderMock.Setup(r => r.Exists(Path.Combine(ImagesRoot, PizzaJpg)))
                        .Returns(true);
        _imageReaderMock.Setup(r => r.Read(Path.Combine(ImagesRoot, PizzaJpg)))
                        .Returns(imageBytes);

        var result = _service.Import(request, AdminEmailCom);

        Assert.AreEqual(1, result.ImportedCount);

        _productServiceMock.Verify(s => s.CreateProduct(
            It.Is<CreateProductRequestDTO>(dto =>
                dto.Code == P0001 &&
                dto.Name == PizzaNapolitana &&
                dto.Description == UnaRicaPizzaNapolitanaConTomat &&
                dto.Price == 250.0 &&
                dto.CommercialLine == Minutas &&
                dto.Category == Pizzas),
            AdminEmailCom), Times.Once);
    }

    [TestMethod]
    public void Import_WhenProductHasImagePaths_ConvertsThemToBase64JoinedByComma()
    {
        var imageBytes1 = new byte[] { 1, 2, 3 };
        var imageBytes2 = new byte[] { 4, 5, 6 };
        var expectedBase64 = $"{Convert.ToBase64String(imageBytes1)},{Convert.ToBase64String(imageBytes2)}";

        var imported = new ImportedProduct
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = UnaRicaPizzaNapolitanaConTomat,
            Price = 250.0,
            CommercialLine = Minutas,
            Category = Pizzas,
            ImagePaths = [Pizza1Jpg, Pizza2Jpg]
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([imported]);
        _providerMock.Setup(p => p.GetByName(Json)).Returns(importerMock.Object);

        _imageReaderMock.Setup(r => r.Exists(Path.Combine(ImagesRoot, Pizza1Jpg))).Returns(true);
        _imageReaderMock.Setup(r => r.Exists(Path.Combine(ImagesRoot, Pizza2Jpg))).Returns(true);
        _imageReaderMock.Setup(r => r.Read(Path.Combine(ImagesRoot, Pizza1Jpg))).Returns(imageBytes1);
        _imageReaderMock.Setup(r => r.Read(Path.Combine(ImagesRoot, Pizza2Jpg))).Returns(imageBytes2);

        var request = new ImportProductsRequestDTO { ImporterName = Json, Content = "x", FileName = "x.json" };

        var result = _service.Import(request, AdminEmailCom);

        Assert.AreEqual(1, result.ImportedCount);
        _productServiceMock.Verify(s => s.CreateProduct(
            It.Is<CreateProductRequestDTO>(dto => dto.Images == expectedBase64),
            AdminEmailCom), Times.Once);
    }

    [TestMethod]
    public void Import_WhenProductHasNoValidImages_AddsToErrorsAndDoesNotCallCreateProduct()
    {
        var imported = new ImportedProduct
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = UnaRicaPizzaNapolitanaConTomat,
            Price = 250.0,
            CommercialLine = Minutas,
            Category = Pizzas,
            ImagePaths = []
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([imported]);
        _providerMock.Setup(p => p.GetByName(Json)).Returns(importerMock.Object);

        var request = new ImportProductsRequestDTO
        {
            ImporterName = Json,
            Content = "x",
            FileName = "x.json"
        };

        var result = _service.Import(request, AdminEmailCom);

        Assert.AreEqual(0, result.ImportedCount);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Se requiere al menos una imagen válida.", result.Errors[0].Reason);

        _productServiceMock.Verify(
            s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>(), It.IsAny<string>()),
            Times.Never);
    }

    [TestMethod]
    public void Import_WhenImagePathIsUrl_DownloadsAndConvertsToBase64()
    {
        var imageBytes = new byte[] { 10, 20, 30 };
        var expectedBase64 = Convert.ToBase64String(imageBytes);
        var imageUrl = "https://ejemplo.com/pizza.jpg";

        var imported = new ImportedProduct
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = UnaRicaPizzaNapolitanaConTomat,
            Price = 250.0,
            CommercialLine = Minutas,
            Category = Pizzas,
            ImagePaths = [imageUrl]
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([imported]);
        _providerMock.Setup(p => p.GetByName(Json)).Returns(importerMock.Object);

        _imageReaderMock.Setup(r => r.DownloadFromUrl(imageUrl)).Returns(imageBytes);

        var request = new ImportProductsRequestDTO { ImporterName = Json, Content = "x", FileName = "x.json" };

        var result = _service.Import(request, AdminEmailCom);

        Assert.AreEqual(1, result.ImportedCount);
        _imageReaderMock.Verify(r => r.DownloadFromUrl(imageUrl), Times.Once);
        _productServiceMock.Verify(s => s.CreateProduct(
            It.Is<CreateProductRequestDTO>(dto => dto.Images == expectedBase64),
            AdminEmailCom), Times.Once);
    }

    [TestMethod]
    public void Import_WhenRowFailsValidation_AddsToErrorsAndDoesNotCallCreateProduct()
    {
        var invalid = new ImportedProduct
        {
            Code = " ",
            Name = PizzaNapolitana,
            Description = UnaRicaPizzaNapolitanaConTomat,
            Price = 250.0,
            CommercialLine = Minutas,
            Category = Pizzas
        };

        var importerMock = new Mock<IProductImporter>();
        importerMock.Setup(i => i.Import(It.IsAny<ImportRequest>())).Returns([invalid]);
        _providerMock.Setup(p => p.GetByName(Json)).Returns(importerMock.Object);

        var request = new ImportProductsRequestDTO
        {
            ImporterName = Json,
            Content = "x",
            FileName = "x.json"
        };

        var result = _service.Import(request, AdminEmailCom);

        Assert.AreEqual(0, result.ImportedCount);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(0, result.Errors[0].Index);
        Assert.AreEqual(" ", result.Errors[0].Code);

        _productServiceMock.Verify(
            s => s.CreateProduct(It.IsAny<CreateProductRequestDTO>(), It.IsAny<string>()),
            Times.Never);
    }
}
