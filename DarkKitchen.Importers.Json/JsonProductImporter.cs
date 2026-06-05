using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Json;

public class JsonProductImporter : IProductImporter
{
    public string Name => "JSON";

    public IEnumerable<ImportedProduct> Import(ImportRequest request)
    {
        throw new NotImplementedException();
    }
}
