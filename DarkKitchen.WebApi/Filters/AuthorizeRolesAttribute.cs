using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeRolesAttribute(params UserRole[] roles) : Attribute, IAuthorizationFilter
{
    private readonly UserRole[] _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = ExtractToken(context);

        if(string.IsNullOrEmpty(token))
        {
            SetUnauthorized(context, "Token requerido");
            return;
        }

        try
        {
            var sessionService = context.HttpContext.RequestServices
                .GetRequiredService<ISessionService>();

            var user = sessionService.GetUserFromToken(token);

            if(!_roles.Contains(user.Role))
            {
                SetForbidden(context, "No tiene permisos");
                return;
            }

            // Agregue esto para guardar
            // el usuario autenticado para que los controllers puedan accederlo
            //  sin necesidad de llamar al servicio de sesión nuevamente
            context.HttpContext.Items["RequestingUser"] = user;
        }
        catch(Exception)
        {
            SetUnauthorized(context, "Token inválido");
        }
    }

    private static string? ExtractToken(AuthorizationFilterContext context)
    {
        return context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
    }

    private static void SetUnauthorized(AuthorizationFilterContext context, string message)
    {
        context.Result = new ObjectResult(message) { StatusCode = 401 };
    }

    private static void SetForbidden(AuthorizationFilterContext context, string message)
    {
        context.Result = new ObjectResult(message) { StatusCode = 403 };
    }
}
