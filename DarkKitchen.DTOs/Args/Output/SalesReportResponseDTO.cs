namespace DarkKitchen.DTOs.Args.Output;

public class SalesReportResponseDTO
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<ClientSalesDTO> Clients { get; set; } = [];
    public double MonthlyTotal { get; set; }
}
