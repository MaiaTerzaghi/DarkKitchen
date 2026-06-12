using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class ClientOnlyAttribute() : AuthorizeRolesAttribute(UserRole.Client)
{
}
