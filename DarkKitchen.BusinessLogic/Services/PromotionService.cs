using DarkKitchen.Domain.Auditing;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository, IRepository<Product> productRepository, IAuditSubject audit) : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IRepository<Product> _productRepository = productRepository;
    private readonly IAuditSubject _audit = audit;

    public List<Promotion> GetActivePromotions(DateTime? date, string? productLine, string? product)
    {
        return _promotionRepository.GetActivePromotions(date, productLine, product);
    }

    public PromotionResponseDTO CreatePromotion(CreatePromotionRequestDTO request, string responsibleUser)
    {
        var promotion = new Promotion
        {
            Name = request.Name,
            DiscountPercentage = request.DiscountPercentage,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };

        var saved = _promotionRepository.Add(promotion);

        _audit.Notify(new AuditEvent
        {
            EntityName = AuditedEntity.Promotion,
            EntityId = saved.Id,
            Description = $"Alta de promoción '{saved.Name}' (id {saved.Id}).",
            ResponsibleUser = responsibleUser
        });

        return new PromotionResponseDTO
        {
            Id = saved.Id,
            Name = saved.Name,
            DiscountPercentage = saved.DiscountPercentage,
            ValidFrom = saved.ValidFrom,
            ValidTo = saved.ValidTo
        };
    }

    public PromotionResponseDTO UpdatePromotion(int id, UpdatePromotionRequestDTO request)
    {
        var promotion = _promotionRepository.Get(p => p.Id == id)
            ?? throw new NotFoundException($"Promoción con id {id} no encontrada.");

        promotion.Name = request.Name;
        promotion.DiscountPercentage = request.DiscountPercentage;
        promotion.ValidFrom = request.ValidFrom;
        promotion.ValidTo = request.ValidTo;

        var updated = _promotionRepository.Update(promotion);

        return new PromotionResponseDTO
        {
            Id = updated.Id,
            Name = updated.Name,
            DiscountPercentage = updated.DiscountPercentage,
            ValidFrom = updated.ValidFrom,
            ValidTo = updated.ValidTo
        };
    }

    public void AddProductToPromotion(int promotionId, int productId)
    {
        var promotion = _promotionRepository.GetPromotionWithProducts(promotionId)
        ?? throw new NotFoundException($"Promoción con id {promotionId} no encontrada.");

        var product = _productRepository.Get(p => p.Id == productId)
            ?? throw new NotFoundException($"Producto con id {productId} no encontrado.");

        if(_promotionRepository.ProductExistsInPromotion(promotionId, productId))
        {
            throw new ArgumentException($"El producto con id {productId} ya está en esta promoción.");
        }

        if(promotion.ValidTo < DateTime.Today)
        {
            throw new ArgumentException($"La promoción con id {promotionId} está vencida.");
        }

        promotion.Products.Add(product);

        _promotionRepository.Update(promotion);
    }

    public void RemoveProductFromPromotion(int promotionId, int productId)
    {
        var promotion = _promotionRepository.GetPromotionWithProducts(promotionId)
        ?? throw new NotFoundException($"Promoción con id {promotionId} no encontrada.");

        var product = promotion.Products.FirstOrDefault(p => p.Id == productId)
            ?? throw new ArgumentException($"El producto con id {productId} no está en la promoción.");

        promotion.Products.Remove(product);

        _promotionRepository.Update(promotion);
    }
}
