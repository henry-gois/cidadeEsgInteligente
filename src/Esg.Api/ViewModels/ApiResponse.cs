namespace Esg.Api.ViewModels;

public sealed record ApiResponse<T>(bool Success, T? Data, Meta Meta, IEnumerable<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully") =>
        new(true, data, new Meta(message, StatusCodes.Status200OK));
}

public sealed record Meta(string Message, int StatusCode);
