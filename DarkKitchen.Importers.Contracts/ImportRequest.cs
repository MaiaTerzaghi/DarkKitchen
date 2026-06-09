namespace DarkKitchen.Importers.Contracts;

public class ImportRequest
{
    public string Content { get; set; } = string.Empty;

    public string? FileName { get; set; }
}
