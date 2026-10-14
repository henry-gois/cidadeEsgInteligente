using System.Security.Claims;
using Esg.Api.Data;
using Esg.Api.Models;
using Esg.Api.Security;
using Esg.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EsgDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnergyConsumptionService, EnergyConsumptionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IIoTAutomationService, IoTAutomationService>();
builder.Services.AddSingleton<ITokenService, HmacTokenService>();

builder.Services.AddAuthentication("HmacBearer")
    .AddScheme<AuthenticationSchemeOptions, HmacAuthenticationHandler>("HmacBearer", _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OperatorOrAdmin", policy =>
        policy.RequireClaim(ClaimTypes.Role, Roles.Operator, Roles.Admin));
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, Roles.Admin));
});

var app = builder.Build();

// In containerized environments, wait for SQL Server and apply pending migrations
// before accepting traffic. This behavior is opt-in via Database:ApplyMigrationsOnStartup.
if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    const int maxAttempts = 12;
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EsgDbContext>();
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully.");
            break;
        }
        catch (Exception exception)
        {
            if (attempt >= maxAttempts)
            {
                app.Logger.LogCritical(exception, "Database migrations failed after {Attempts} attempts.", attempt);
                throw;
            }

            app.Logger.LogWarning(exception,
                "Database is not ready for migrations (attempt {Attempt}/{MaxAttempts}). Retrying in 5 seconds.",
                attempt, maxAttempts);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
if (!builder.Configuration.GetValue<bool>("Hosting:DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Readiness endpoint used by Docker Compose and the deployment pipeline.
app.MapGet("/health", async (EsgDbContext dbContext, CancellationToken cancellationToken) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? Results.Ok(new { status = "healthy" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}).AllowAnonymous();

app.Run();

public partial class Program
{
}
