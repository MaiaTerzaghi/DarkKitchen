using System.Xml.Linq;
using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Xml;

public class XmlProductImporter : IProductImporter
{
    public string Name => "XML";

    public IEnumerable<ImportedProduct> Import(ImportRequest request)
    {
        var doc = XDocument.Parse(request.Content);

        return doc.Descendants("Product").Select(p => new ImportedProduct
        {
            Code = p.Element("Code")?.Value ?? string.Empty,
            Name = p.Element("Name")?.Value ?? string.Empty,
            Description = p.Element("Description")?.Value ?? string.Empty,
            Price = double.Parse(p.Element("Price")?.Value ?? "0"),
            CommercialLine = p.Element("CommercialLine")?.Value ?? string.Empty,
            Category = p.Element("Category")?.Value ?? string.Empty,
            ImagePaths = p.Element("ImagePaths")?.Elements("Path")
                .Select(e => e.Value).ToList() ?? [],
        });
    }
}
