using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esg.Api.Controllers;

[ApiController]
[Route("energy-consumptions")]
public sealed class EnergyConsumptionController(IEnergyConsumptionService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "OperatorOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnergyConsumptionViewModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnergyConsumptionViewModel>>>> GetAll(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PagedResult<EnergyConsumptionViewModel>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "OperatorOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EnergyConsumptionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<EnergyConsumptionViewModel>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EnergyConsumptionViewModel>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<EnergyConsumptionViewModel>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<EnergyConsumptionViewModel>>> Create(
        [FromBody] UpsertEnergyConsumptionViewModel request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EnergyConsumptionViewModel>.Ok(result, "Energy consumption created successfully"));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<EnergyConsumptionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<EnergyConsumptionViewModel>>> Update(
        long id,
        [FromBody] UpsertEnergyConsumptionViewModel request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<EnergyConsumptionViewModel>.Ok(result, "Energy consumption updated successfully"));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Energy consumption deleted successfully"));
    }
}
