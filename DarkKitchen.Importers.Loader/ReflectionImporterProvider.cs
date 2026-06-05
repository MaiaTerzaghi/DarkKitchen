using DarkKitchen.IBusinessLogic;
using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Loader;

public sealed class ReflectionImporterProvider(string pluginsPath) : IImporterProvider
{
    private readonly string _pluginsPath = pluginsPath;

    public IReadOnlyCollection<string> GetImporterNames()
    {
        if (!Directory.Exists(_pluginsPath))
        {
            return [];
        }

        return [];
    }

    public IProductImporter GetByName(string name)
    {
        throw new NotImplementedException();
    }
}
