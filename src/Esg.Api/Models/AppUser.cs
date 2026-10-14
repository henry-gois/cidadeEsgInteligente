namespace Esg.Api.Models;

public sealed class AppUser
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Operator;
}
