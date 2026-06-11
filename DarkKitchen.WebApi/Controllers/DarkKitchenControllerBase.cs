using DarkKitchen.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

public abstract class DarkKitchenControllerBase : ControllerBase
{
    protected User GetRequestingUser()
    {
        return (User)HttpContext.Items["RequestingUser"]!;
    }
}
