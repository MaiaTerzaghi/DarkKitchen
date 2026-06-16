using DarkKitchen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class GlobalExceptionFilterAttribute : Attribute, IExceptionFilter
{
    private readonly Dictionary<Type, int> _statusCodes = new()
    {
        { typeof(NotFoundException), StatusCodes.Status404NotFound },
        { typeof(ConflictException), StatusCodes.Status409Conflict },
        { typeof(UnauthorizedException), StatusCodes.Status401Unauthorized },
        { typeof(ArgumentException), StatusCodes.Status400BadRequest }
    };

    public void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;

        var statusCode = _statusCodes.GetValueOrDefault(
            context.Exception.GetType(),
            StatusCodes.Status500InternalServerError);

        var message = statusCode == StatusCodes.Status500InternalServerError
            ? "Error inesperado en el servidor."
            : context.Exception.Message;

        context.Result = new ObjectResult(new { message })
        {
            StatusCode = statusCode
        };
    }
}
