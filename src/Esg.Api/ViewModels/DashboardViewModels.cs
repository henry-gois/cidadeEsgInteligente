namespace Esg.Api.ViewModels;

public sealed record DashboardIndicatorsViewModel(
    decimal TotalKwhCurrentMonth,
    decimal AverageKwhPerUnit,
    int OpenAlerts,
    int IdleDevices,
    decimal EstimatedCo2Kg);

public sealed record EfficiencyRankingViewModel(long UnitId, string UnitName, string City, decimal KwhCurrentMonth);

public sealed record MonthlyConsumptionViewModel(int Year, int Month, decimal TotalKwh);

public sealed record ConsumptionByUnitViewModel(long UnitId, string UnitName, decimal TotalKwh);
