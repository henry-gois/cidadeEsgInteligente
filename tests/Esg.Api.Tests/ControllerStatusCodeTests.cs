using Esg.Api.Controllers;
using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Esg.Api.Tests;

public sealed class ControllerStatusCodeTests
{
    [Fact]
    public async Task AuthenticationController_Login_ReturnsStatusCode200()
    {
        var controller = new AuthenticationController(new AuthServiceStub());

        var response = await controller.Login(new LoginRequest("admin", "admin123"), CancellationToken.None);

        AssertOk(response.Result);
    }

    [Fact]
    public async Task EnergyConsumptionController_GetAll_ReturnsStatusCode200()
    {
        var controller = new EnergyConsumptionController(new EnergyConsumptionServiceStub());

        var response = await controller.GetAll(new PageRequest(), CancellationToken.None);

        AssertOk(response.Result);
    }

    [Fact]
    public async Task DashboardController_GetIndicators_ReturnsStatusCode200()
    {
        var controller = new DashboardController(new DashboardServiceStub());

        var response = await controller.GetIndicators(CancellationToken.None);

        AssertOk(response.Result);
    }

    [Fact]
    public async Task AlertController_GetAll_ReturnsStatusCode200()
    {
        var controller = new AlertController(new AlertServiceStub());

        var response = await controller.GetAll(new PageRequest(), CancellationToken.None);

        AssertOk(response.Result);
    }

    [Fact]
    public async Task IoTAutomationController_GetDeviceIdleTime_ReturnsStatusCode200()
    {
        var controller = new IoTAutomationController(new IoTAutomationServiceStub());

        var response = await controller.GetDeviceIdleTime(1, CancellationToken.None);

        AssertOk(response.Result);
    }

    private static void AssertOk(IActionResult? result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    private sealed class AuthServiceStub : IAuthService
    {
        public Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TokenResponse("token", "Bearer", DateTime.UtcNow.AddHours(1), "ADMIN"));
        }
    }

    private sealed class EnergyConsumptionServiceStub : IEnergyConsumptionService
    {
        public Task<PagedResult<EnergyConsumptionViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<EnergyConsumptionViewModel> items =
            [
                new(1, 1, "Unidade Paulista", 1000, new DateOnly(2026, 5, 1))
            ];
            return Task.FromResult(new PagedResult<EnergyConsumptionViewModel>(items, 1, 10, 1, 1));
        }

        public Task<EnergyConsumptionViewModel> GetByIdAsync(long id, CancellationToken cancellationToken) =>
            Task.FromResult(new EnergyConsumptionViewModel(id, 1, "Unidade Paulista", 1000, new DateOnly(2026, 5, 1)));

        public Task<EnergyConsumptionViewModel> CreateAsync(UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken) =>
            GetByIdAsync(1, cancellationToken);

        public Task<EnergyConsumptionViewModel> UpdateAsync(long id, UpsertEnergyConsumptionViewModel request, CancellationToken cancellationToken) =>
            GetByIdAsync(id, cancellationToken);

        public Task DeleteAsync(long id, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class DashboardServiceStub : IDashboardService
    {
        public Task<DashboardIndicatorsViewModel> GetIndicatorsAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new DashboardIndicatorsViewModel(1000, 500, 1, 2, 84));

        public Task<PagedResult<EfficiencyRankingViewModel>> GetEfficiencyRankingAsync(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<EfficiencyRankingViewModel> items =
            [
                new(1, "Unidade Paulista", "Sao Paulo", 1000)
            ];
            return Task.FromResult(new PagedResult<EfficiencyRankingViewModel>(items, 1, 10, 1, 1));
        }

        public Task<PagedResult<MonthlyConsumptionViewModel>> GetMonthlyConsumptionAsync(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<MonthlyConsumptionViewModel> items =
            [
                new(2026, 5, 1000)
            ];
            return Task.FromResult(new PagedResult<MonthlyConsumptionViewModel>(items, 1, 10, 1, 1));
        }

        public Task<PagedResult<ConsumptionByUnitViewModel>> GetConsumptionByUnitAsync(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ConsumptionByUnitViewModel> items =
            [
                new(1, "Unidade Paulista", 1000)
            ];
            return Task.FromResult(new PagedResult<ConsumptionByUnitViewModel>(items, 1, 10, 1, 1));
        }
    }

    private sealed class AlertServiceStub : IAlertService
    {
        public Task<PagedResult<AlertViewModel>> GetAllAsync(PageRequest pageRequest, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<AlertViewModel> items =
            [
                new(1, 1, "Unidade Paulista", DateTime.UtcNow, "Consumo acima da meta.", "OPEN")
            ];
            return Task.FromResult(new PagedResult<AlertViewModel>(items, 1, 10, 1, 1));
        }

        public Task<AlertViewModel> GetByIdAsync(long id, CancellationToken cancellationToken) =>
            Task.FromResult(new AlertViewModel(id, 1, "Unidade Paulista", DateTime.UtcNow, "Consumo acima da meta.", "OPEN"));

        public Task<PagedResult<AlertViewModel>> GetByUnitAsync(long unitId, PageRequest pageRequest, CancellationToken cancellationToken) =>
            GetAllAsync(pageRequest, cancellationToken);

        public Task<OpenAlertsViewModel> GetOpenAlertsCountAsync(long unitId, CancellationToken cancellationToken) =>
            Task.FromResult(new OpenAlertsViewModel(unitId, 1));

        public Task<int> GenerateAlertsAsync(CancellationToken cancellationToken) => Task.FromResult(1);

        public Task<AlertViewModel> CloseAsync(long id, CancellationToken cancellationToken) =>
            Task.FromResult(new AlertViewModel(id, 1, "Unidade Paulista", DateTime.UtcNow, "Consumo acima da meta.", "CLOSED"));
    }

    private sealed class IoTAutomationServiceStub : IIoTAutomationService
    {
        public Task<ShutdownIdleDevicesViewModel> ShutdownIdleDevicesAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new ShutdownIdleDevicesViewModel(1, DateTime.UtcNow));

        public Task<IdleTimeViewModel> GetDeviceIdleTimeAsync(long deviceId, CancellationToken cancellationToken) =>
            Task.FromResult(new IdleTimeViewModel(deviceId, "HVAC Bloco A", 9.5, "STANDBY"));

        public Task<IdleDevicesCountViewModel> GetIdleDevicesCountAsync(long unitId, CancellationToken cancellationToken) =>
            Task.FromResult(new IdleDevicesCountViewModel(unitId, 1));
    }
}
