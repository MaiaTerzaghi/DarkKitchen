using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportController(IImportService importService) : ControllerBase
{
    private readonly IImportService _importService = importService;

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("importers")]
    public IActionResult GetAvailableImporters()
    {
        var importers = _importService.GetAvailableImporters();
        return Ok(importers);
    }
}
