using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportController(IImportService importService) : DarkKitchenControllerBase
{
    private readonly IImportService _importService = importService;

    [AdministrativeOnly]
    [HttpGet("importers")]
    public IActionResult GetAvailableImporters()
    {
        var importers = _importService.GetAvailableImporters();
        return Ok(importers);
    }

    [AdministrativeOnly]
    [HttpPost]
    public IActionResult Import([FromBody] ImportProductsRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var result = _importService.Import(request, requestingUser.Email);
        return Ok(result);
    }
}
