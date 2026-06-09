using DarkKitchen.Domain.Validators;
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
        var index = 0;

        foreach (var imported in importer.Import(importerRequest))
        {
            var error = FindValidationError(imported);
            if (error != null)
            {
                result.Errors.Add(new ImportErrorDTO
                {
                    Index = index,
                    Code = imported.Code,
                    Reason = error
                });
                index++;
                continue;
            }

            var dto = BuildDto(imported);

            if (string.IsNullOrWhiteSpace(dto.Images))
            {
                result.Errors.Add(new ImportErrorDTO
                {
                    Index = index,
                    Code = imported.Code,
                    Reason = "Se requiere al menos una imagen válida."
                });
                index++;
                continue;
            }

            _productService.CreateProduct(dto, responsibleUser);
            result.ImportedCount++;

            index++;
        }

        return result;
    }

    private CreateProductRequestDTO BuildDto(ImportedProduct imported)
    {
        return new CreateProductRequestDTO
        {
            Code = imported.Code,
            Name = imported.Name,
            Description = imported.Description,
            Price = imported.Price,
            CommercialLine = imported.CommercialLine,
            Category = imported.Category,
            Images = ConvertImagesToBase64(imported.ImagePaths)
        };
    }

    private string ConvertImagesToBase64(List<string> paths)
    {
        var base64s = new List<string>();

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            byte[] bytes;

            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                bytes = _imageReader.DownloadFromUrl(path);
            }
            else
            {
                var fullPath = Path.Combine(_imagesRoot, path);
                if (!_imageReader.Exists(fullPath))
                {
                    continue;
                }

                bytes = _imageReader.Read(fullPath);
            }

            base64s.Add(Convert.ToBase64String(bytes));
        }

        return string.Join(",", base64s);
    }

    private static string? FindValidationError(ImportedProduct imported)
    {
        try
        {
            ProductValidator.ValidateCode(imported.Code);
            ProductValidator.ValidateName(imported.Name);
            ProductValidator.ValidateDescription(imported.Description);
            ProductValidator.ValidatePrice(imported.Price);
            ProductValidator.ValidateCommercialLine(imported.CommercialLine);
            ProductValidator.ValidateCategory(imported.Category);
            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}
