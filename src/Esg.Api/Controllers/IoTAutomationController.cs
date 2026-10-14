using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esg.Api.Controllers;

[ApiController]
[Route("iot")]
public sealed class IoTAutomationController(IIoTAutomationService service) : ControllerBase
{
    [HttpPost("shutdown-idle")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ShutdownIdleDevicesViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ShutdownIdleDevicesViewModel>>> ShutdownIdleDevices(CancellationToken cancellationToken)
    {
        var result = await service.ShutdownIdleDevicesAsync(cancellationToken);
        return Ok(ApiResponse<ShutdownIdleDevicesViewModel>.Ok(result, "Idle devices shutdown completed"));
    }

    [HttpGet("devices/{deviceId:long}/idle-time")]
    [Authorize(Policy = "OperatorOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IdleTimeViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IdleTimeViewModel>>> GetDeviceIdleTime(long deviceId, CancellationToken cancellationToken)
    {
        var result = await service.GetDeviceIdleTimeAsync(deviceId, cancellationToken);
        return Ok(ApiResponse<IdleTimeViewModel>.Ok(result));
    }

    [HttpGet("units/{unitId:long}/idle-devices/count")]
    [Authorize(Policy = "OperatorOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IdleDevicesCountViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IdleDevicesCountViewModel>>> GetIdleDevicesCount(long unitId, CancellationToken cancellationToken)
    {
        var result = await service.GetIdleDevicesCountAsync(unitId, cancellationToken);
        return Ok(ApiResponse<IdleDevicesCountViewModel>.Ok(result));
    }
}
