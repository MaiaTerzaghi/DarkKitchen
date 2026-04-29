using DarkKitchen.Domain.Enums;
namespace DarkKitchen.DTOs.Args.In;

public class GetClientOrdersRequestDTO
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public OrderStatus? Status { get; set; }
}
