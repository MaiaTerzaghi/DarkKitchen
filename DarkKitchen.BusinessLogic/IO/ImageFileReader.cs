using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.IO;

public class ImageFileReader : IImageFileReader
{
    public bool Exists(string fullPath) => File.Exists(fullPath);

    public byte[] Read(string fullPath) => File.ReadAllBytes(fullPath);
}
