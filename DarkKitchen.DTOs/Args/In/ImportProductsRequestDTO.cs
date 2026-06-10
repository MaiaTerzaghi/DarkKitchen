namespace DarkKitchen.DTOs.Args.In;

public class ImportProductsRequestDTO
{
    public string ImporterName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? FileName { get; set; }
}
