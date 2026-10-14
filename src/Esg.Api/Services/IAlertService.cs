using Esg.Api.ViewModels;

namespace Esg.Api.Services;

public interface IAlertService
{
    Task<PagedResult<AlertViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken);
    Task<AlertViewModel> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<PagedResult<AlertViewModel>> GetByUnitAsync(long unitId, PageRequest pageRequest, CancellationToken cancellationToken);
    Task<OpenAlertsViewModel> GetOpenAlertsCountAsync(long unitId, CancellationToken cancellationToken);
    Task<int> GenerateAlertsAsync(CancellationToken cancellationToken);
    Task<AlertViewModel> CloseAsync(long id, CancellationToken cancellationToken);
}
