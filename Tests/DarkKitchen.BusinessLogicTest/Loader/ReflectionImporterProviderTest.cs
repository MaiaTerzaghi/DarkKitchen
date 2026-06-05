using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Importers.Loader;

namespace DarkKitchen.BusinessLogicTest.Loader;

[TestClass]
public sealed class ReflectionImporterProviderTest
{
    [TestMethod]
    public void GetImporterNames_WhenPluginsFolderDoesNotExist_ReturnsEmpty()
    {
        var provider = new ReflectionImporterProvider("/non/existent/path");

        var result = provider.GetImporterNames();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetImporterNames_WhenFolderHasValidPlugin_ReturnsImporterName()
    {
        var testBinDir = Path.GetDirectoryName(typeof(TestProductImporter).Assembly.Location)!;

        var provider = new ReflectionImporterProvider(testBinDir);

        var result = provider.GetImporterNames();

        Assert.IsTrue(result.Contains("TEST"));
    }

    [TestMethod]
    public void GetByName_WhenValidPluginExists_ReturnsImporterInstance()
    {
        var testBinDir = Path.GetDirectoryName(typeof(TestProductImporter).Assembly.Location)!;
        var provider = new ReflectionImporterProvider(testBinDir);

        var result = provider.GetByName("TEST");

        Assert.IsNotNull(result);
        Assert.AreEqual("TEST", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void GetByName_WhenImporterDoesNotExist_ThrowsNotFoundException()
    {
        var testBinDir = Path.GetDirectoryName(typeof(TestProductImporter).Assembly.Location)!;
        var provider = new ReflectionImporterProvider(testBinDir);

        provider.GetByName("DOESNOTEXIST");
    }
}
