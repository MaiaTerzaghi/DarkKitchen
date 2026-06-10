using DarkKitchen.BusinessLogic.IO;

namespace DarkKitchen.BusinessLogicTest.IO;

[TestClass]
public sealed class ImageFileReaderTest
{
    [TestMethod]
    public void Read_WhenFileExists_ReturnsFileBytes()
    {
        var tempFile = Path.GetTempFileName();
        var expectedBytes = new byte[] { 1, 2, 3, 4, 5 };
        File.WriteAllBytes(tempFile, expectedBytes);

        var reader = new ImageFileReader();

        var result = reader.Read(tempFile);

        CollectionAssert.AreEqual(expectedBytes, result);

        File.Delete(tempFile);
    }
}
