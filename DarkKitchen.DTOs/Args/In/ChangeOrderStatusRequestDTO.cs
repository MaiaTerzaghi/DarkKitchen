using DarkKitchen.Domain.Enums;

namespace DarkKitchen.DTOs.Args.In;

public class ChangeOrderStatusRequestDTO
{
    public OrderStatus Status { get; set; }
}
