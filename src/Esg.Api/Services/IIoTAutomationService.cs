using Esg.Api.ViewModels;

namespace Esg.Api.Services;

public interface IIoTAutomationService
{
    Task<ShutdownIdleDevicesViewModel> ShutdownIdleDevicesAsync(CancellationToken cancellationToken);
    Task<IdleTimeViewModel> GetDeviceIdleTimeAsync(long deviceId, CancellationToken cancellationToken);
    Task<IdleDevicesCountViewModel> GetIdleDevicesCountAsync(long unitId, CancellationToken cancellationToken);
}
