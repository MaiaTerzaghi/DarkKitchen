namespace DarkKitchen.DTOs.Args.Output;

public class ImportErrorDTO
{
    public int Index { get; set; }

    public string? Code { get; set; }

    public string Reason { get; set; } = string.Empty;
}
