using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public class ExceptionFilterTest
{
    private ExceptionContext CreateContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var context = new ExceptionContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            [])
        {
            Exception = exception
        };
        return context;
    }

    [TestMethod]
    public void OnException_WhenArgumentException_Returns400()
    {
        var filter = new ExceptionFilter();
        var context = CreateContext(new ArgumentException("Error"));

        filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.IsTrue(context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_WhenUnknownException_Returns500()
    {
        var filter = new ExceptionFilter();
        var context = CreateContext(new Exception("Error inesperado"));

        filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(500, result.StatusCode);
        Assert.IsTrue(context.ExceptionHandled);
    }
}
