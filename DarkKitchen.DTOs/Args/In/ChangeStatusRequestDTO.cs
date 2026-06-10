using DarkKitchen.Domain.Enums;

namespace DarkKitchen.DTOs.Args.In;

public class ChangeStatusRequestDTO
{
    public OrderStatus Status { get; set; }
}
