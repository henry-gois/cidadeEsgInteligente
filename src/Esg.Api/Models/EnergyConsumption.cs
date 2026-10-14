namespace Esg.Api.Models;

public sealed class EnergyConsumption
{
    public long Id { get; set; }
    public long UnitId { get; set; }
    public Unit? Unit { get; set; }
    public decimal KwhConsumed { get; set; }
    public DateOnly ReferenceMonth { get; set; }
}
