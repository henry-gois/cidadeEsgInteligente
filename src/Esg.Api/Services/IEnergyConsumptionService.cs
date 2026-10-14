using Esg.Api.ViewModels;

namespace Esg.Api.Services;

public interface IEnergyConsumptionService
{
    Task<PagedResult<EnergyConsumptionViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken);
    Task<EnergyConsumptionViewModel> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EnergyConsumptionViewModel> CreateAsync(UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken);
    Task<EnergyConsumptionViewModel> UpdateAsync(long id, UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
}
