using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public class AuthorizeRolesAttributeTest
{
    [TestMethod]
    public void OnAuthorization_WhenNoToken_Returns401()
    {
        var sessionServiceMock = new Mock<ISessionService>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => sessionServiceMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var context = new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            []);

        var filter = new ClientOnlyAttribute();

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_WhenInvalidToken_Returns401()
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock
            .Setup(s => s.GetUserFromToken(It.IsAny<string>()))
            .Throws(new Exception());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => sessionServiceMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        httpContext.Request.Headers["Authorization"] = "invalid-token";

        var context = new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            []);

        var filter = new ClientOnlyAttribute();

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserDoesNotHaveRole_Returns403()
    {
        var user = new User { Id = 1, Role = UserRole.Client };

        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock
            .Setup(s => s.GetUserFromToken(It.IsAny<string>()))
            .Returns(user);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => sessionServiceMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        httpContext.Request.Headers["Authorization"] = "valid-token";

        var context = new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            []);

        // El endpoint requiere Admin, pero el user es Client
        var filter = new AdministrativeOnlyAttribute();

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserHasCorrectRole_AllowsAccess()
    {
        var user = new User { Id = 1, Role = UserRole.Client };

        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock
            .Setup(s => s.GetUserFromToken(It.IsAny<string>()))
            .Returns(user);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => sessionServiceMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        httpContext.Request.Headers["Authorization"] = "valid-token";

        var context = new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            []);

        var filter = new ClientOnlyAttribute();

        filter.OnAuthorization(context);

        // Si pasa, NO debería setear Result (queda null)
        Assert.IsNull(context.Result);
    }
}
