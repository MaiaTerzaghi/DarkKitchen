using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(DarkKitchenContext context) : IOrderRepository
{
    // TDD Red: se suprime el warning de campo no utilizado temporalmente.
    // _context será utilizado en el paso Green cuando  mplemente Save.
#pragma warning disable CA1823, CS9113
    private readonly DarkKitchenContext _context = context;
#pragma warning restore CA1823, CS9113

    public Order Save(Order order)
    {
        throw new NotImplementedException();
    }
}
