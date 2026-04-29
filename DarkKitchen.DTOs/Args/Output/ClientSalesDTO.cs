namespace DarkKitchen.DTOs.Args.Output;

public class ClientSalesDTO
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public double Total { get; set; }
}
