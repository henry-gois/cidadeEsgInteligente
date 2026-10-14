namespace Esg.Api.Models;

public sealed class Device
{
    public long Id { get; set; }
    public long UnitId { get; set; }
    public Unit? Unit { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public decimal RatedPowerW { get; set; }
    public string Status { get; set; } = DeviceStatuses.Online;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
}
