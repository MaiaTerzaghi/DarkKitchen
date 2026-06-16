using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class AdministrativeOnlyAttribute() : AuthorizeRolesAttribute(UserRole.Administrative)
{
}
