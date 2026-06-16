namespace DarkKitchen.IBusinessLogic;

public interface IImageFileReader
{
    bool Exists(string fullPath);

    byte[] Read(string fullPath);

    byte[] DownloadFromUrl(string url);
}
