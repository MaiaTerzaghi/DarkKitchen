using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;
public interface IPromotionService
{
    PaginatedResponse<PromotionResponseDTO> GetActivePromotions(DateTime? date, string? productLine, string? product, int page = 1, int pageSize = 20);
    PromotionResponseDTO CreatePromotion(CreatePromotionRequestDTO request, string responsibleUser);
    PromotionResponseDTO UpdatePromotion(int id, UpdatePromotionRequestDTO request, string responsibleUser);
    void AddProductToPromotion(int promotionId, int productId);
    void RemoveProductFromPromotion(int promotionId, int productId);
}
