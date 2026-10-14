using System.ComponentModel.DataAnnotations;

namespace Esg.Api.ViewModels;

public sealed record EnergyConsumptionViewModel(
    long Id,
    long UnitId,
    string? UnitName,
    decimal KwhConsumed,
    DateOnly ReferenceMonth);

public sealed record UpsertEnergyConsumptionViewModel(
    [Range(1, long.MaxValue)] long UnitId,
    [Range(typeof(decimal), "0.01", "999999.99")] decimal KwhConsumed,
    DateOnly ReferenceMonth);
