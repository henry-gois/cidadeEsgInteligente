namespace Esg.Api.ViewModels;

public sealed record IdleTimeViewModel(long DeviceId, string DeviceName, double IdleHours, string Status);

public sealed record IdleDevicesCountViewModel(long UnitId, int Count);

public sealed record ShutdownIdleDevicesViewModel(int DevicesUpdated, DateTime ExecutedAt);
