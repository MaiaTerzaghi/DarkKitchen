namespace DarkKitchen.DTOs.Args.Output;

public class SalesReportWithTotalDTO
{
    public List<SalesReportResponseDTO> Months { get; set; } = [];
    public double GeneralTotal { get; set; }
}
