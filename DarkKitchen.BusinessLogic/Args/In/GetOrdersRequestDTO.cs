namespace DarkKitchen.BusinessLogic.Args.In;

public class GetOrdersRequestDTO
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string? Street { get; set; }
    public string? Status { get; set; }
}
