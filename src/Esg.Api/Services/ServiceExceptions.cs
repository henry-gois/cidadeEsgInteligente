namespace Esg.Api.Services;

public sealed class NotFoundException(string message) : Exception(message);

public sealed class BusinessException(string message) : Exception(message);
