using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Entities;

public class Promotion
{
    private string _name = string.Empty;
    private decimal _discountPercentage;
    private DateTime _validFrom;
    private DateTime _validTo;

    public int Id { get; set; }
    public string ProductLine { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
    public string Name
    {
        get => _name;
        set
        {
            PromotionValidator.ValidateName(value);
            _name = value;
        }
    }

    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            PromotionValidator.ValidateDiscountPercentage(value);
            _discountPercentage = value;
        }
    }

    public DateTime ValidFrom
    {
        get => _validFrom;
        set => _validFrom = value;
    }

    public DateTime ValidTo
    {
        get => _validTo;
        set
        {
            PromotionValidator.ValidateDateRange(_validFrom, value);
            _validTo = value;
        }
    }
}
