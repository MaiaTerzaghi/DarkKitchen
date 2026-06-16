using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class DispatcherOnlyAttribute() : AuthorizeRolesAttribute(UserRole.Dispatcher)
{
}
