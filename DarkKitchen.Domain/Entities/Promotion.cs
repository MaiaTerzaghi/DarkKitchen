namespace DarkKitchen.Domain.Entities;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string ProductLine { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
}
