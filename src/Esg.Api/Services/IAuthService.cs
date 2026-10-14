using Esg.Api.ViewModels;

namespace Esg.Api.Services;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
