namespace DarkKitchen.Importers.Contracts;

public interface IProductImporter
{
    string Name { get; }

    IEnumerable<ImportedProduct> Import(ImportRequest request);
}
