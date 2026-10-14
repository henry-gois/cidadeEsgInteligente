using System.ComponentModel.DataAnnotations;

namespace Esg.Api.ViewModels;

public sealed record LoginRequest(
    [Required, StringLength(80)] string UserName,
    [Required, StringLength(80, MinimumLength = 4)] string Password);

public sealed record TokenResponse(string AccessToken, string TokenType, DateTime ExpiresAt, string Role);
