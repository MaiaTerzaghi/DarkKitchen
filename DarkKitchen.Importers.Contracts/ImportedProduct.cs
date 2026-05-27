namespace DarkKitchen.Importers.Contracts;

public class ImportedProduct
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Price { get; set; }

    public string CommercialLine { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<string> ImagePaths { get; set; } = new();
}
