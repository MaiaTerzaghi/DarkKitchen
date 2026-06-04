using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Importers.Contracts;

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
        var importer = _importerProvider.GetByName(request.ImporterName);
        var importerRequest = new ImportRequest
        {
            Content = request.Content,
            FileName = request.FileName
        };

        var result = new ImportResultDTO();

        foreach (var imported in importer.Import(importerRequest))
        {
            var dto = new CreateProductRequestDTO
            {
                Code = imported.Code,
                Name = imported.Name,
                Description = imported.Description,
                Price = imported.Price,
                CommercialLine = imported.CommercialLine,
                Category = imported.Category,
                Images = ConvertImagesToBase64(imported.ImagePaths)
            };
            _productService.CreateProduct(dto, responsibleUser);
            result.ImportedCount++;
        }

        return result;
    }

    private string ConvertImagesToBase64(List<string> paths)
    {
        var base64s = paths
            .Select(p => Path.Combine(_imagesRoot, p))
            .Select(_imageReader.Read)
            .Select(Convert.ToBase64String);

        return string.Join(",", base64s);
    }
}
