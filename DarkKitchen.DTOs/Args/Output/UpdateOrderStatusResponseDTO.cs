namespace DarkKitchen.DTOs.Args.Output;

public class UpdateOrderStatusResponseDTO
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
