using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esg.Api.Controllers;

[ApiController]
[Route("alerts")]
[Authorize(Policy = "OperatorOrAdmin")]
public sealed class AlertController(IAlertService alertService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlertViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AlertViewModel>>>> GetAll(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await alertService.GetAllAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<AlertViewModel>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<AlertViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AlertViewModel>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await alertService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<AlertViewModel>.Ok(result));
    }

    [HttpGet("unit/{unitId:long}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlertViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AlertViewModel>>>> GetByUnit(
        long unitId,
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await alertService.GetByUnitAsync(unitId, pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<AlertViewModel>>.Ok(result));
    }

    [HttpGet("unit/{unitId:long}/open-count")]
    [ProducesResponseType(typeof(ApiResponse<OpenAlertsViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OpenAlertsViewModel>>> GetOpenAlertsCount(long unitId, CancellationToken cancellationToken)
    {
        var result = await alertService.GetOpenAlertsCountAsync(unitId, cancellationToken);
        return Ok(ApiResponse<OpenAlertsViewModel>.Ok(result));
    }

    [HttpPost("generate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GenerateAlerts(CancellationToken cancellationToken)
    {
        var created = await alertService.GenerateAlertsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { created }, "Alerts generated successfully"));
    }

    [HttpPatch("{id:long}/close")]
    [ProducesResponseType(typeof(ApiResponse<AlertViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AlertViewModel>>> Close(long id, CancellationToken cancellationToken)
    {
        var result = await alertService.CloseAsync(id, cancellationToken);
        return Ok(ApiResponse<AlertViewModel>.Ok(result, "Alert closed successfully"));
    }
}
