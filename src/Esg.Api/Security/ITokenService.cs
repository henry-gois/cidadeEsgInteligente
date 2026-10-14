using Esg.Api.ViewModels;

namespace Esg.Api.Security;

public interface ITokenService
{
    TokenResponse CreateToken(string userName, string role);
    TokenPrincipal? ValidateToken(string token);
}

public sealed record TokenPrincipal(string UserName, string Role, DateTime ExpiresAt);
