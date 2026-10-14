namespace Esg.Api.Models;

public sealed class ConsumptionAlert
{
    public long Id { get; set; }
    public long UnitId { get; set; }
    public Unit? Unit { get; set; }
    public DateTime AlertDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = AlertStatuses.Open;
}
