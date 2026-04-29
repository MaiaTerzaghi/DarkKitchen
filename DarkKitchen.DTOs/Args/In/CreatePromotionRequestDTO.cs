namespace DarkKitchen.DTOs.Args.In;

public class CreatePromotionRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
