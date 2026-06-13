using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class AnyRoleAttribute() : AuthorizeRolesAttribute(UserRole.Client, UserRole.Administrative, UserRole.Dispatcher)
{
}
