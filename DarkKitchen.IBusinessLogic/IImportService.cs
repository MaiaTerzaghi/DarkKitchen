using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;

public interface IImportService
{
    IReadOnlyCollection<ImporterInfoDTO> GetAvailableImporters();

    ImportResultDTO Import(ImportProductsRequestDTO request, string responsibleUser);
}
