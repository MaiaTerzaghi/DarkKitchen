using DarkKitchen.Importers.Contracts;
using DarkKitchen.Importers.Json;

namespace DarkKitchen.Importers.Json.Test;

[TestClass]
public sealed class JsonProductImporterTest
{
    private JsonProductImporter _importer = null!;

    [TestInitialize]
    public void Setup()
    {
        _importer = new JsonProductImporter();
    }

    [TestMethod]
    public void Name_ReturnsJson()
    {
        Assert.AreEqual("JSON", _importer.Name);
    }
}
