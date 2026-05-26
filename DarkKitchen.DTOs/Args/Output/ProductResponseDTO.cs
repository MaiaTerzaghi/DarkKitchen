namespace DarkKitchen.DTOs.Args.Output;

public class ProductResponseDTO
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string CommercialLine { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Images { get; set; } = string.Empty;
}
