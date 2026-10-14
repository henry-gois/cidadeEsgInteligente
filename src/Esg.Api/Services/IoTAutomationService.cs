using Esg.Api.Data;
using Esg.Api.Models;
using Esg.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Esg.Api.Services;

public sealed class IoTAutomationService(EsgDbContext dbContext) : IIoTAutomationService
{
    private static readonly TimeSpan IdleThreshold = TimeSpan.FromHours(8);

    public async Task<ShutdownIdleDevicesViewModel> ShutdownIdleDevicesAsync(CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.Subtract(IdleThreshold);
        var devices = await dbContext.Devices
            .Where(device => device.Status != DeviceStatuses.Offline && device.LastActivityAt <= threshold)
            .ToListAsync(cancellationToken);

        foreach (var device in devices)
        {
            device.Status = DeviceStatuses.Offline;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ShutdownIdleDevicesViewModel(devices.Count, DateTime.UtcNow);
    }

    public async Task<IdleTimeViewModel> GetDeviceIdleTimeAsync(long deviceId, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == deviceId, cancellationToken)
            ?? throw new NotFoundException("Device was not found.");

        var idleHours = Math.Max(0, DateTime.UtcNow.Subtract(device.LastActivityAt).TotalHours);
        return new IdleTimeViewModel(device.Id, device.Name, Math.Round(idleHours, 2), device.Status);
    }

    public async Task<IdleDevicesCountViewModel> GetIdleDevicesCountAsync(long unitId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Units.AnyAsync(unit => unit.Id == unitId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Unit was not found.");
        }

        var threshold = DateTime.UtcNow.Subtract(IdleThreshold);
        var count = await dbContext.Devices
            .CountAsync(device => device.UnitId == unitId && device.Status != DeviceStatuses.Offline && device.LastActivityAt <= threshold, cancellationToken);

        return new IdleDevicesCountViewModel(unitId, count);
    }
}
