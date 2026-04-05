using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeRolesAttribute(params UserRole[] roles) : Attribute, IAuthorizationFilter
{
    private readonly UserRole[] _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if(string.IsNullOrEmpty(token))
        {
            context.Result = new ObjectResult("Token requerido") { StatusCode = 401 };
            return;
        }

        try
        {
            var sessionService = context.HttpContext.RequestServices
                .GetRequiredService<ISessionService>();

            var user = sessionService.GetUserFromToken(token);

            if(!_roles.Contains(user.Role))
            {
                context.Result = new ObjectResult("No tiene permisos") { StatusCode = 403 };
            }
        }
        catch(Exception)
        {
            context.Result = new ObjectResult("Token inválido") { StatusCode = 401 };
        }
    }
}
