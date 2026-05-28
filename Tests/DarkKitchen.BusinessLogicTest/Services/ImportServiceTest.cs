using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
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
}
