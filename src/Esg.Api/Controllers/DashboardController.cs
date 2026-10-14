using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esg.Api.Controllers;

[ApiController]
[Route("dashboard")]
[Authorize(Policy = "OperatorOrAdmin")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("indicators")]
    [ProducesResponseType(typeof(ApiResponse<DashboardIndicatorsViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardIndicatorsViewModel>>> GetIndicators(CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetIndicatorsAsync(cancellationToken);
        return Ok(ApiResponse<DashboardIndicatorsViewModel>.Ok(result));
    }

    [HttpGet("efficiency-ranking")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EfficiencyRankingViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<EfficiencyRankingViewModel>>>> GetEfficiencyRanking(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetEfficiencyRankingAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<EfficiencyRankingViewModel>>.Ok(result));
    }

    [HttpGet("monthly-consumption")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MonthlyConsumptionViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<MonthlyConsumptionViewModel>>>> GetMonthlyConsumption(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetMonthlyConsumptionAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<MonthlyConsumptionViewModel>>.Ok(result));
    }

    [HttpGet("consumption-by-unit")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ConsumptionByUnitViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsumptionByUnitViewModel>>>> GetConsumptionByUnit(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetConsumptionByUnitAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<ConsumptionByUnitViewModel>>.Ok(result));
    }
}
