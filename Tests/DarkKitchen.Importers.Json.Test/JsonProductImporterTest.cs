using System.Text.Json;
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

    [TestMethod]
    public void Import_ValidJson_ReturnsProductsWithCorrectFields()
    {
        var json = @"[
            {
                ""code"": ""P001"",
                ""name"": ""Pizza Margherita"",
                ""description"": ""Pizza clasica con tomate y mozzarella"",
                ""price"": 350.0,
                ""commercialLine"": ""Pizzas"",
                ""category"": ""Fritos"",
                ""imagePaths"": [""pizza1.jpg""]
            }
        ]";

        var request = new ImportRequest { Content = json, FileName = "productos.json" };
        var result = _importer.Import(request).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("P001", result[0].Code);
        Assert.AreEqual("Pizza Margherita", result[0].Name);
        Assert.AreEqual("Pizza clasica con tomate y mozzarella", result[0].Description);
        Assert.AreEqual(350.0, result[0].Price);
        Assert.AreEqual("Pizzas", result[0].CommercialLine);
        Assert.AreEqual("Fritos", result[0].Category);
        Assert.AreEqual(1, result[0].ImagePaths.Count);
        Assert.AreEqual("pizza1.jpg", result[0].ImagePaths[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Import_MalformedJson_ThrowsJsonException()
    {
        var request = new ImportRequest { Content = "esto no es json {{{", FileName = "bad.json" };
        _importer.Import(request).ToList();
    }

    [TestMethod]
    public void Import_EmptyArray_ReturnsEmpty()
    {
        var request = new ImportRequest { Content = "[]", FileName = "empty.json" };
        var result = _importer.Import(request).ToList();

        Assert.AreEqual(0, result.Count);
    }
}
