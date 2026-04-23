using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(DarkKitchenContext context) : Repository<Product>(context), IProductRepository
{
}
