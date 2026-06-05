using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.BusinessLogicTest.Loader;

public class TestProductImporter : IProductImporter
{
    public string Name => "TEST";

    public IEnumerable<ImportedProduct> Import(ImportRequest request) => [];
}
