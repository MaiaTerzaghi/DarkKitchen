using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class AuditControllerTest
{
    [TestMethod]
    public void GetLogs_WhenCalled_ReturnsOk()
    {
        var paginatedResponse = new PaginatedResponse<AuditLogResponseDTO>
        {
            Items = [],
            TotalCount = 0,
            Page = 1,
            PageSize = 20
        };

        var auditServiceMock = new Mock<IAuditService>();
        auditServiceMock.Setup(s => s.GetLogs(It.IsAny<GetAuditLogsRequestDTO>()))
            .Returns(paginatedResponse);
        var controller = new AuditController(auditServiceMock.Object);

        var result = controller.GetLogs(new GetAuditLogsRequestDTO());

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
}
