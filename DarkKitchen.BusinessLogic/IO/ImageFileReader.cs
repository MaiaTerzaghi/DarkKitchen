using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.IO;

public class ImageFileReader : IImageFileReader
{
    public byte[] Read(string fullPath) => File.ReadAllBytes(fullPath);
}
