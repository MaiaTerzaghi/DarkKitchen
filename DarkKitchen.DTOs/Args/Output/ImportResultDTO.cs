namespace DarkKitchen.DTOs.Args.Output;

public class ImportResultDTO
{
    public int ImportedCount { get; set; }

    public List<ImportErrorDTO> Errors { get; set; } = [];
}
