using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.IO;

public class ImageFileReader : IImageFileReader
{
    private static readonly HttpClient _httpClient = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "DarkKitchen/1.0");
        return client;
    }

    public bool Exists(string fullPath) => File.Exists(fullPath);

    public byte[] Read(string fullPath) => File.ReadAllBytes(fullPath);

    public byte[] DownloadFromUrl(string url) => _httpClient.GetByteArrayAsync(url).Result;
}
