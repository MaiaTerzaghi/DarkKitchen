namespace DarkKitchen.DTOs.Args.In;

public class GetProductsManageRequestDTO : PaginationParamsDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? CommercialLine { get; set; }
    public bool? IsActive { get; set; }
    public double? PriceMin { get; set; }
    public double? PriceMax { get; set; }
}
