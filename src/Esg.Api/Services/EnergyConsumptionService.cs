using Esg.Api.Data;
using Esg.Api.Models;
using Esg.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Esg.Api.Services;

public sealed class EnergyConsumptionService(EsgDbContext dbContext) : IEnergyConsumptionService
{
    public Task<PagedResult<EnergyConsumptionViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return dbContext.EnergyConsumptions
            .AsNoTracking()
            .OrderByDescending(consumption => consumption.ReferenceMonth)
            .ThenBy(consumption => consumption.Unit!.Name)
            .Select(consumption => new EnergyConsumptionViewModel(
                consumption.Id,
                consumption.UnitId,
                consumption.Unit!.Name,
                consumption.KwhConsumed,
                consumption.ReferenceMonth))
            .ToPagedResultAsync(pageRequest, cancellationToken);
    }

    public async Task<EnergyConsumptionViewModel> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var result = await dbContext.EnergyConsumptions
            .AsNoTracking()
            .Where(consumption => consumption.Id == id)
            .Select(consumption => new EnergyConsumptionViewModel(
                consumption.Id,
                consumption.UnitId,
                consumption.Unit!.Name,
                consumption.KwhConsumed,
                consumption.ReferenceMonth))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? throw new NotFoundException("Energy consumption was not found.");
    }

    public async Task<EnergyConsumptionViewModel> CreateAsync(UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken)
    {
        await EnsureUnitExistsAsync(request.UnitId, cancellationToken);
        await EnsureMonthIsAvailableAsync(null, request.UnitId, request.ReferenceMonth, cancellationToken);

        var consumption = new EnergyConsumption
        {
            UnitId = request.UnitId,
            KwhConsumed = request.KwhConsumed,
            ReferenceMonth = new DateOnly(request.ReferenceMonth.Year, request.ReferenceMonth.Month, 1)
        };

        dbContext.EnergyConsumptions.Add(consumption);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(consumption.Id, cancellationToken);
    }

    public async Task<EnergyConsumptionViewModel> UpdateAsync(long id, UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken)
    {
        var consumption = await dbContext.EnergyConsumptions.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException("Energy consumption was not found.");

        await EnsureUnitExistsAsync(request.UnitId, cancellationToken);
        await EnsureMonthIsAvailableAsync(id, request.UnitId, request.ReferenceMonth, cancellationToken);

        consumption.UnitId = request.UnitId;
        consumption.KwhConsumed = request.KwhConsumed;
        consumption.ReferenceMonth = new DateOnly(request.ReferenceMonth.Year, request.ReferenceMonth.Month, 1);

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var consumption = await dbContext.EnergyConsumptions.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException("Energy consumption was not found.");

        dbContext.EnergyConsumptions.Remove(consumption);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUnitExistsAsync(long unitId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Units.AnyAsync(unit => unit.Id == unitId, cancellationToken);
        if (!exists)
        {
            throw new BusinessException("The informed unit does not exist.");
        }
    }

    private async Task EnsureMonthIsAvailableAsync(long? id, long unitId, DateOnly referenceMonth, CancellationToken cancellationToken)
    {
        var normalizedMonth = new DateOnly(referenceMonth.Year, referenceMonth.Month, 1);
        var exists = await dbContext.EnergyConsumptions.AnyAsync(consumption =>
            consumption.UnitId == unitId &&
            consumption.ReferenceMonth == normalizedMonth &&
            (!id.HasValue || consumption.Id != id.Value), cancellationToken);

        if (exists)
        {
            throw new BusinessException("This unit already has consumption registered for the informed month.");
        }
    }
}
