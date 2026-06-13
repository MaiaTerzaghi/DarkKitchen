using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class AdministrativeOrDispatcherAttribute() : AuthorizeRolesAttribute(UserRole.Administrative, UserRole.Dispatcher)
{
}
