namespace Esg.Api.Models;

public sealed class Unit
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public ICollection<Device> Devices { get; set; } = [];
    public ICollection<EnergyConsumption> Consumptions { get; set; } = [];
    public ICollection<ConsumptionAlert> Alerts { get; set; } = [];
}
