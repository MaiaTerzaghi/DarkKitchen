namespace DarkKitchen.BusinessLogic.Args.In;

public class GetClientOrdersRequestDTO
{
    public int ClientId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Status { get; set; }
}
