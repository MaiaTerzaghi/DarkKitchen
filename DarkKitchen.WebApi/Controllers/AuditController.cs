using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/audit-logs")]
public class AuditController(IAuditService auditService) : ControllerBase
{
    private readonly IAuditService _auditService = auditService;

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet]
    public IActionResult GetLogs([FromQuery] GetAuditLogsRequestDTO request)
    {
        var logs = _auditService.GetLogs(request);
        return Ok(logs);
    }
}
