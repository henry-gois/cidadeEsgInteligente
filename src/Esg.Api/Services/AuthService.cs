using System.Security.Cryptography;
using System.Text;
using Esg.Api.Data;
using Esg.Api.Security;
using Esg.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Esg.Api.Services;

public sealed class AuthService(EsgDbContext dbContext, ITokenService tokenService) : IAuthService
{
    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var passwordHash = HashPassword(request.Password);
        var user = await dbContext.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserName == request.UserName && item.PasswordHash == passwordHash, cancellationToken);

        if (user is null)
        {
            throw new BusinessException("Invalid credentials.");
        }

        return tokenService.CreateToken(user.UserName, user.Role);
    }

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
