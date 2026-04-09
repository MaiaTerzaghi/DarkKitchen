namespace DarkKitchen.BusinessLogic.Args.Output;

public class GetClientOrdersResponseDTO
{
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public double Total { get; set; }
    public int ItemCount { get; set; }
}
