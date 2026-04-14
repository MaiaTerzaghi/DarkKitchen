using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IPromotionService
{
    List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product);
    PromotionResponseDTO CreatePromotion(CreatePromotionRequestDTO request);
}
