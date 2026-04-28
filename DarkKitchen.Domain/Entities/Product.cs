using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Entities;

public class Product
{
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _commercialLine = string.Empty;
    private string _category = string.Empty;
    private double _price;
    private string _images = string.Empty;
    public bool IsActive { get; set; } = true;

    public int Id { get; set; }

    public string Code
    {
        get => _code;
        set
        {
            ProductValidator.ValidateCode(value);
            _code = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            ProductValidator.ValidateName(value);
            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            ProductValidator.ValidateDescription(value);
            _description = value;
        }
    }

    public double Price
    {
        get => _price;
        set
        {
            ProductValidator.ValidatePrice(value);
            _price = value;
        }
    }

    public string CommercialLine
    {
        get => _commercialLine;
        set
        {
            ProductValidator.ValidateCommercialLine(value);
            _commercialLine = value;
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            ProductValidator.ValidateCategory(value);
            _category = value;
        }
    }

    public string Images
    {
        get => _images;
        set
        {
            ProductValidator.ValidateImages(value);
            _images = value;
        }
    }
}
