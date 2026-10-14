using Esg.Api.Models;
using Esg.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using Esg.Api.Data;

namespace Esg.Api.Services;

public sealed class DashboardService(EsgDbContext dbContext) : IDashboardService
{
    public async Task<DashboardIndicatorsViewModel> GetIndicatorsAsync(CancellationToken cancellationToken)
    {
        var referenceMonth = await GetLatestReferenceMonthAsync(cancellationToken);

        var currentMonthQuery = dbContext.EnergyConsumptions
            .AsNoTracking()
            .Where(consumption => consumption.ReferenceMonth == referenceMonth);

        var totalKwh = await currentMonthQuery.SumAsync(consumption => consumption.KwhConsumed, cancellationToken);
        var unitCount = await dbContext.Units.CountAsync(cancellationToken);
        var openAlerts = await dbContext.ConsumptionAlerts.CountAsync(alert => alert.Status == AlertStatuses.Open, cancellationToken);
        var idleDevices = await dbContext.Devices.CountAsync(device => device.Status == DeviceStatuses.Standby, cancellationToken);
        var average = unitCount == 0 ? 0 : decimal.Round(totalKwh / unitCount, 2);

        return new DashboardIndicatorsViewModel(totalKwh, average, openAlerts, idleDevices, decimal.Round(totalKwh * 0.084m, 2));
    }

    public async Task<PagedResult<EfficiencyRankingViewModel>> GetEfficiencyRankingAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var referenceMonth = await GetLatestReferenceMonthAsync(cancellationToken);

        return await dbContext.EnergyConsumptions
            .AsNoTracking()
            .Where(consumption => consumption.ReferenceMonth == referenceMonth)
            .OrderBy(consumption => consumption.KwhConsumed)
            .Select(consumption => new EfficiencyRankingViewModel(
                consumption.UnitId,
                consumption.Unit!.Name,
                consumption.Unit.City,
                consumption.KwhConsumed))
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    public async Task<PagedResult<MonthlyConsumptionViewModel>> GetMonthlyConsumptionAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return await dbContext.EnergyConsumptions
            .AsNoTracking()
            .GroupBy(consumption => consumption.ReferenceMonth)
            .OrderBy(group => group.Key)
            .Select(group => new MonthlyConsumptionViewModel(group.Key.Year, group.Key.Month, group.Sum(consumption => consumption.KwhConsumed)))
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    public async Task<PagedResult<ConsumptionByUnitViewModel>> GetConsumptionByUnitAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return await dbContext.EnergyConsumptions
            .AsNoTracking()
            .GroupBy(consumption => new { consumption.UnitId, consumption.Unit!.Name })
            .OrderByDescending(group => group.Sum(consumption => consumption.KwhConsumed))
            .Select(group => new ConsumptionByUnitViewModel(group.Key.UnitId, group.Key.Name, group.Sum(consumption => consumption.KwhConsumed)))
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    private async Task<DateOnly> GetLatestReferenceMonthAsync(CancellationToken cancellationToken)
    {
        var month = await dbContext.EnergyConsumptions
            .AsNoTracking()
            .OrderByDescending(consumption => consumption.ReferenceMonth)
            .Select(consumption => (DateOnly?)consumption.ReferenceMonth)
            .FirstOrDefaultAsync(cancellationToken);

        return month ?? new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    }
}
