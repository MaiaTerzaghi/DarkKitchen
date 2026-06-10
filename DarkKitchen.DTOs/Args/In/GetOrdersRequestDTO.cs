using DarkKitchen.Domain.Enums;
namespace DarkKitchen.DTOs.Args.In;

public class GetOrdersRequestDTO : PaginationParamsDTO
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Street { get; set; }
    public OrderStatus? Status { get; set; }
}
