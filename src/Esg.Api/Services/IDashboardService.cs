using Esg.Api.ViewModels;

namespace Esg.Api.Services;

public interface IDashboardService
{
    Task<DashboardIndicatorsViewModel> GetIndicatorsAsync(CancellationToken cancellationToken);
    Task<PagedResult<EfficiencyRankingViewModel>> GetEfficiencyRankingAsync(PageRequest pageRequest, CancellationToken cancellationToken);
    Task<PagedResult<MonthlyConsumptionViewModel>> GetMonthlyConsumptionAsync(PageRequest pageRequest, CancellationToken cancellationToken);
    Task<PagedResult<ConsumptionByUnitViewModel>> GetConsumptionByUnitAsync(PageRequest pageRequest, CancellationToken cancellationToken);
}
