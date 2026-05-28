using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Services;

public class ImportService(
    IImporterProvider importerProvider,
    IProductService productService,
    IImageFileReader imageReader,
    string imagesRoot) : IImportService
{
    private readonly IImporterProvider _importerProvider = importerProvider;
    private readonly IProductService _productService = productService;
    private readonly IImageFileReader _imageReader = imageReader;
    private readonly string _imagesRoot = imagesRoot;

    public IReadOnlyCollection<ImporterInfoDTO> GetAvailableImporters()
    {
        return _importerProvider.GetImporterNames()
            .Select(name => new ImporterInfoDTO { Name = name })
            .ToList();
    }

    public ImportResultDTO Import(ImportProductsRequestDTO request, string responsibleUser)
    {
        throw new NotImplementedException();
    }
}
