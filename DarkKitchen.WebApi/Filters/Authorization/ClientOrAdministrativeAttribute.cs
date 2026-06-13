using DarkKitchen.Domain.Enums;
namespace DarkKitchen.WebApi.Filters.Authorization;
public sealed class ClientOrAdministrativeAttribute() : AuthorizeRolesAttribute(UserRole.Client, UserRole.Administrative)
{
}
