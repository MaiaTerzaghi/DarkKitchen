namespace DarkKitchen.DTOs.Args.Output;

public class TopProductResponseDTO
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Images { get; set; } = string.Empty;
}
