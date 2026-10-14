using Esg.Api.Data;
using Esg.Api.Models;
using Esg.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Esg.Api.Services;

public sealed class AlertService(EsgDbContext dbContext) : IAlertService
{
    private const decimal MonthlyLimitKwh = 14000m;

    public Task<PagedResult<AlertViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return ProjectAlerts(dbContext.ConsumptionAlerts.AsNoTracking())
            .OrderByDescending(alert => alert.AlertDate)
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    public async Task<AlertViewModel> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var alert = await ProjectAlerts(dbContext.ConsumptionAlerts.AsNoTracking().Where(item => item.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

        return alert ?? throw new NotFoundException("Alert was not found.");
    }

    public Task<PagedResult<AlertViewModel>> GetByUnitAsync(long unitId, PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return ProjectAlerts(dbContext.ConsumptionAlerts.AsNoTracking().Where(alert => alert.UnitId == unitId))
            .OrderByDescending(alert => alert.AlertDate)
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    public async Task<OpenAlertsViewModel> GetOpenAlertsCountAsync(long unitId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Units.AnyAsync(unit => unit.Id == unitId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Unit was not found.");
        }

        var count = await dbContext.ConsumptionAlerts
            .CountAsync(alert => alert.UnitId == unitId && alert.Status == AlertStatuses.Open, cancellationToken);

        return new OpenAlertsViewModel(unitId, count);
    }

    public async Task<int> GenerateAlertsAsync(CancellationToken cancellationToken)
    {
        var excessiveConsumptions = await dbContext.EnergyConsumptions
            .AsNoTracking()
            .Where(consumption => consumption.KwhConsumed >= MonthlyLimitKwh)
            .Select(consumption => new
            {
                consumption.UnitId,
                consumption.ReferenceMonth,
                consumption.KwhConsumed,
                UnitName = consumption.Unit!.Name
            })
            .ToListAsync(cancellationToken);

        var created = 0;
        foreach (var item in excessiveConsumptions)
        {
            var message = $"{item.UnitName} consumed {item.KwhConsumed:n2} kWh in {item.ReferenceMonth:yyyy-MM}.";
            var exists = await dbContext.ConsumptionAlerts.AnyAsync(alert =>
                alert.UnitId == item.UnitId &&
                alert.Status == AlertStatuses.Open &&
                alert.Message == message, cancellationToken);

            if (exists)
            {
                continue;
            }

            dbContext.ConsumptionAlerts.Add(new ConsumptionAlert
            {
                UnitId = item.UnitId,
                AlertDate = DateTime.UtcNow,
                Message = message,
                Status = AlertStatuses.Open
            });
            created++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return created;
    }

    public async Task<AlertViewModel> CloseAsync(long id, CancellationToken cancellationToken)
    {
        var alert = await dbContext.ConsumptionAlerts.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException("Alert was not found.");

        alert.Status = AlertStatuses.Closed;
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    private static IQueryable<AlertViewModel> ProjectAlerts(IQueryable<ConsumptionAlert> query)
    {
        return query.Select(alert => new AlertViewModel(
            alert.Id,
            alert.UnitId,
            alert.Unit!.Name,
            alert.AlertDate,
            alert.Message,
            alert.Status));
    }
}
