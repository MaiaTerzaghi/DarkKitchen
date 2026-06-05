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
}
