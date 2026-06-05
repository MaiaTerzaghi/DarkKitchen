using System.Text.Json;
using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Json;

public class JsonProductImporter : IProductImporter
{
    public string Name => "JSON";

    public IEnumerable<ImportedProduct> Import(ImportRequest request)
    {
        var products = JsonSerializer.Deserialize<List<ImportedProduct>>(request.Content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("El archivo JSON no contiene productos válidos.");

        return products;
    }
}
