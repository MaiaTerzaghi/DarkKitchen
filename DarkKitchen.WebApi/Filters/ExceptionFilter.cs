using DarkKitchen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if(context.Exception is NotFoundException)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            return;
        }

        if(context.Exception is ConflictException)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = StatusCodes.Status409Conflict
            };
            return;
        }

        if(context.Exception is UnauthorizedException)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        if(context.Exception is ArgumentException)
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
