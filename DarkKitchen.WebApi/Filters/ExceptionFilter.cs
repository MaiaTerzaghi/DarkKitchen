using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[ExcludeFromCodeCoverage]
public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ArgumentException)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            return;
        }

        context.ExceptionHandled = true;
        context.Result = new ObjectResult(new { message = "Error inesperado en el servidor." })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
