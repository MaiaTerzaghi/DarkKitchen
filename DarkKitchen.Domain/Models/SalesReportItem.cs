namespace DarkKitchen.Domain.Models;

public class SalesReportItem
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public double Total { get; set; }
}
