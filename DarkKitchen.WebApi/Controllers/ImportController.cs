using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportController(IImportService importService) : DarkKitchenControllerBase
{
    private readonly IImportService _importService = importService;

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("importers")]
    public IActionResult GetAvailableImporters()
    {
        var importers = _importService.GetAvailableImporters();
        return Ok(importers);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult Import([FromBody] ImportProductsRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var result = _importService.Import(request, requestingUser.Email);
        return Ok(result);
    }
}
