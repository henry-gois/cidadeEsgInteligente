using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Esg.Api.ViewModels;

namespace Esg.Api.Security;

public sealed class HmacTokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _issuer = configuration["Auth:Issuer"] ?? "EsgEnergy.Api";
    private readonly string _audience = configuration["Auth:Audience"] ?? "EsgEnergy.Client";
    private readonly byte[] _key = Encoding.UTF8.GetBytes(configuration["Auth:SigningKey"] ?? "change-this-development-key");

    public TokenResponse CreateToken(string userName, string role)
    {
        var expiresAt = DateTime.UtcNow.AddHours(2);
        var header = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new { alg = "HS256", typ = "JWT" }));
        var payload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new
        {
            sub = userName,
            role,
            iss = _issuer,
            aud = _audience,
            exp = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
        }));
        var signature = Sign($"{header}.{payload}");

        return new TokenResponse($"{header}.{payload}.{signature}", "Bearer", expiresAt, role);
    }

    public TokenPrincipal? ValidateToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        var expectedSignature = Sign($"{parts[0]}.{parts[1]}");
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[2])))
        {
            return null;
        }

        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
        using var document = JsonDocument.Parse(payloadJson);
        var root = document.RootElement;

        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(root.GetProperty("exp").GetInt64()).UtcDateTime;
        if (expiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        if (root.GetProperty("iss").GetString() != _issuer || root.GetProperty("aud").GetString() != _audience)
        {
            return null;
        }

        return new TokenPrincipal(
            root.GetProperty("sub").GetString() ?? string.Empty,
            root.GetProperty("role").GetString() ?? string.Empty,
            expiresAt);
    }

    private string Sign(string value)
    {
        using var hmac = new HMACSHA256(_key);
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }
}
