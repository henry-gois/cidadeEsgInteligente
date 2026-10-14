using Esg.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Esg.Api.Data;

public sealed class EsgDbContext(DbContextOptions<EsgDbContext> options) : DbContext(options)
{
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<EnergyConsumption> EnergyConsumptions => Set<EnergyConsumption>();
    public DbSet<ConsumptionAlert> ConsumptionAlerts => Set<ConsumptionAlert>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("units");
            entity.HasKey(unit => unit.Id);
            entity.Property(unit => unit.Name).HasMaxLength(120).IsRequired();
            entity.Property(unit => unit.City).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("devices");
            entity.HasKey(device => device.Id);
            entity.Property(device => device.Name).HasMaxLength(120).IsRequired();
            entity.Property(device => device.DeviceType).HasMaxLength(80).IsRequired();
            entity.Property(device => device.RatedPowerW).HasPrecision(12, 2);
            entity.Property(device => device.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(device => new { device.UnitId, device.Status });
            entity.HasOne(device => device.Unit)
                .WithMany(unit => unit.Devices)
                .HasForeignKey(device => device.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EnergyConsumption>(entity =>
        {
            entity.ToTable("energy_consumptions");
            entity.HasKey(consumption => consumption.Id);
            entity.Property(consumption => consumption.KwhConsumed).HasPrecision(12, 2);
            entity.HasIndex(consumption => new { consumption.UnitId, consumption.ReferenceMonth }).IsUnique();
            entity.HasOne(consumption => consumption.Unit)
                .WithMany(unit => unit.Consumptions)
                .HasForeignKey(consumption => consumption.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConsumptionAlert>(entity =>
        {
            entity.ToTable("consumption_alerts");
            entity.HasKey(alert => alert.Id);
            entity.Property(alert => alert.Message).HasMaxLength(300).IsRequired();
            entity.Property(alert => alert.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(alert => new { alert.UnitId, alert.Status, alert.AlertDate });
            entity.HasOne(alert => alert.Unit)
                .WithMany(unit => unit.Alerts)
                .HasForeignKey(alert => alert.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("app_users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.UserName).HasMaxLength(80).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(128).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(20).IsRequired();
            entity.HasIndex(user => user.UserName).IsUnique();
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>().HasData(
            new { Id = 1L, Name = "Unidade Paulista", City = "Sao Paulo" },
            new { Id = 2L, Name = "Unidade Vila Olimpia", City = "Sao Paulo" },
            new { Id = 3L, Name = "Unidade Alphaville", City = "Barueri" });

        modelBuilder.Entity<Device>().HasData(
            new { Id = 1L, UnitId = 1L, Name = "HVAC Bloco A", DeviceType = "HVAC", RatedPowerW = 18000m, Status = DeviceStatuses.Online, LastActivityAt = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc) },
            new { Id = 2L, UnitId = 1L, Name = "Iluminacao Terreo", DeviceType = "LIGHTING", RatedPowerW = 4200m, Status = DeviceStatuses.Standby, LastActivityAt = new DateTime(2026, 5, 30, 22, 0, 0, DateTimeKind.Utc) },
            new { Id = 3L, UnitId = 2L, Name = "Data Center", DeviceType = "IT", RatedPowerW = 26000m, Status = DeviceStatuses.Online, LastActivityAt = new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<EnergyConsumption>().HasData(
            new { Id = 1L, UnitId = 1L, KwhConsumed = 14820m, ReferenceMonth = new DateOnly(2026, 4, 1) },
            new { Id = 2L, UnitId = 1L, KwhConsumed = 15140m, ReferenceMonth = new DateOnly(2026, 5, 1) },
            new { Id = 3L, UnitId = 2L, KwhConsumed = 11210m, ReferenceMonth = new DateOnly(2026, 5, 1) },
            new { Id = 4L, UnitId = 3L, KwhConsumed = 7840m, ReferenceMonth = new DateOnly(2026, 5, 1) });

        modelBuilder.Entity<ConsumptionAlert>().HasData(
            new { Id = 1L, UnitId = 1L, AlertDate = new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc), Message = "Consumo mensal acima da meta definida.", Status = AlertStatuses.Open });

        modelBuilder.Entity<AppUser>().HasData(
            new { Id = 1L, UserName = "admin", PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", Role = Roles.Admin },
            new { Id = 2L, UserName = "operator", PasswordHash = "ec6e1c25258002eb1c67d15c7f45da7945fa4c58778fd7d88faa5e53e3b4698d", Role = Roles.Operator });
    }
}
