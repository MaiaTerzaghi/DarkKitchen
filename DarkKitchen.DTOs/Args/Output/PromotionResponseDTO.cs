namespace DarkKitchen.DTOs.Args.Output;

public class PromotionResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string ProductLine { get; set; } = string.Empty;
    public List<PromotionProductDTO> Products { get; set; } = [];
}

public class PromotionProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
