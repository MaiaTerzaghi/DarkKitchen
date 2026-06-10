using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.IBusinessLogic;

public interface IImporterProvider
{
    IReadOnlyCollection<string> GetImporterNames();

    IProductImporter GetByName(string name);
}
