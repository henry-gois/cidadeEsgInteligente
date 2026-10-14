namespace Esg.Api.ViewModels;

public sealed record AlertViewModel(long Id, long UnitId, string UnitName, DateTime AlertDate, string Message, string Status);

public sealed record OpenAlertsViewModel(long UnitId, int Count);
