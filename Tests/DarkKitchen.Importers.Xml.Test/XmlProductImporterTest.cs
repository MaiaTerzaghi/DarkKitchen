using System.Xml;
using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Xml.Test;

[TestClass]
public sealed class XmlProductImporterTest
{
    private XmlProductImporter _importer = null!;

    [TestInitialize]
    public void Setup()
    {
        _importer = new XmlProductImporter();
    }

    [TestMethod]
    public void Name_ReturnsXml()
    {
        Assert.AreEqual("XML", _importer.Name);
    }

    [TestMethod]
    public void Import_ValidXml_ReturnsProductsWithCorrectFields()
    {
        var xml = @"<Products>
            <Product>
                <Code>P001</Code>
                <Name>Pizza Margherita</Name>
                <Description>Pizza clasica con tomate y mozzarella</Description>
                <Price>350</Price>
                <CommercialLine>Pizzas</CommercialLine>
                <Category>Fritos</Category>
                <ImagePaths>
                    <Path>pizza1.jpg</Path>
                </ImagePaths>
            </Product>
        </Products>";

        var request = new ImportRequest { Content = xml, FileName = "productos.xml" };
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
    [ExpectedException(typeof(XmlException))]
    public void Import_MalformedXml_ThrowsXmlException()
    {
        var request = new ImportRequest { Content = "esto no es xml <<<", FileName = "bad.xml" };
        _importer.Import(request).ToList();
    }

    [TestMethod]
    public void Import_EmptyProducts_ReturnsEmpty()
    {
        var request = new ImportRequest { Content = "<Products></Products>", FileName = "empty.xml" };
        var result = _importer.Import(request).ToList();

        Assert.AreEqual(0, result.Count);
    }
}
