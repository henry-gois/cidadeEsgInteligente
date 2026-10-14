using System;
using Esg.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Esg.Api.Migrations;

[DbContext(typeof(EsgDbContext))]
public partial class EsgDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Esg.Api.Models.AppUser", entity =>
        {
            entity.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<long>("Id"));

            entity.Property<string>("PasswordHash")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnType("nvarchar(128)");

            entity.Property<string>("Role")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            entity.Property<string>("UserName")
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnType("nvarchar(80)");

            entity.HasKey("Id");
            entity.HasIndex("UserName").IsUnique();
            entity.ToTable("app_users");

            entity.HasData(
                new { Id = 1L, PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", Role = "ADMIN", UserName = "admin" },
                new { Id = 2L, PasswordHash = "ec6e1c25258002eb1c67d15c7f45da7945fa4c58778fd7d88faa5e53e3b4698d", Role = "OPERATOR", UserName = "operator" });
        });

        modelBuilder.Entity("Esg.Api.Models.ConsumptionAlert", entity =>
        {
            entity.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<long>("Id"));

            entity.Property<DateTime>("AlertDate").HasColumnType("datetime2");
            entity.Property<string>("Message").IsRequired().HasMaxLength(300).HasColumnType("nvarchar(300)");
            entity.Property<string>("Status").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
            entity.Property<long>("UnitId").HasColumnType("bigint");

            entity.HasKey("Id");
            entity.HasIndex("UnitId", "Status", "AlertDate");
            entity.ToTable("consumption_alerts");

            entity.HasData(new
            {
                Id = 1L,
                AlertDate = new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc),
                Message = "Consumo mensal acima da meta definida.",
                Status = "OPEN",
                UnitId = 1L
            });
        });

        modelBuilder.Entity("Esg.Api.Models.Device", entity =>
        {
            entity.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<long>("Id"));

            entity.Property<string>("DeviceType").IsRequired().HasMaxLength(80).HasColumnType("nvarchar(80)");
            entity.Property<DateTime>("LastActivityAt").HasColumnType("datetime2");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
            entity.Property<decimal>("RatedPowerW").HasPrecision(12, 2).HasColumnType("decimal(12,2)");
            entity.Property<string>("Status").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
            entity.Property<long>("UnitId").HasColumnType("bigint");

            entity.HasKey("Id");
            entity.HasIndex("UnitId", "Status");
            entity.ToTable("devices");

            entity.HasData(
                new { Id = 1L, DeviceType = "HVAC", LastActivityAt = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc), Name = "HVAC Bloco A", RatedPowerW = 18000m, Status = "ONLINE", UnitId = 1L },
                new { Id = 2L, DeviceType = "LIGHTING", LastActivityAt = new DateTime(2026, 5, 30, 22, 0, 0, DateTimeKind.Utc), Name = "Iluminacao Terreo", RatedPowerW = 4200m, Status = "STANDBY", UnitId = 1L },
                new { Id = 3L, DeviceType = "IT", LastActivityAt = new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc), Name = "Data Center", RatedPowerW = 26000m, Status = "ONLINE", UnitId = 2L });
        });

        modelBuilder.Entity("Esg.Api.Models.EnergyConsumption", entity =>
        {
            entity.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<long>("Id"));

            entity.Property<decimal>("KwhConsumed").HasPrecision(12, 2).HasColumnType("decimal(12,2)");
            entity.Property<DateOnly>("ReferenceMonth").HasColumnType("date");
            entity.Property<long>("UnitId").HasColumnType("bigint");

            entity.HasKey("Id");
            entity.HasIndex("UnitId", "ReferenceMonth").IsUnique();
            entity.ToTable("energy_consumptions");

            entity.HasData(
                new { Id = 1L, KwhConsumed = 14820m, ReferenceMonth = new DateOnly(2026, 4, 1), UnitId = 1L },
                new { Id = 2L, KwhConsumed = 15140m, ReferenceMonth = new DateOnly(2026, 5, 1), UnitId = 1L },
                new { Id = 3L, KwhConsumed = 11210m, ReferenceMonth = new DateOnly(2026, 5, 1), UnitId = 2L },
                new { Id = 4L, KwhConsumed = 7840m, ReferenceMonth = new DateOnly(2026, 5, 1), UnitId = 3L });
        });

        modelBuilder.Entity("Esg.Api.Models.Unit", entity =>
        {
            entity.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<long>("Id"));

            entity.Property<string>("City").IsRequired().HasMaxLength(80).HasColumnType("nvarchar(80)");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");

            entity.HasKey("Id");
            entity.ToTable("units");

            entity.HasData(
                new { Id = 1L, City = "Sao Paulo", Name = "Unidade Paulista" },
                new { Id = 2L, City = "Sao Paulo", Name = "Unidade Vila Olimpia" },
                new { Id = 3L, City = "Barueri", Name = "Unidade Alphaville" });
        });

        modelBuilder.Entity("Esg.Api.Models.ConsumptionAlert", entity =>
        {
            entity.HasOne("Esg.Api.Models.Unit", "Unit")
                .WithMany("Alerts")
                .HasForeignKey("UnitId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Navigation("Unit");
        });

        modelBuilder.Entity("Esg.Api.Models.Device", entity =>
        {
            entity.HasOne("Esg.Api.Models.Unit", "Unit")
                .WithMany("Devices")
                .HasForeignKey("UnitId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Navigation("Unit");
        });

        modelBuilder.Entity("Esg.Api.Models.EnergyConsumption", entity =>
        {
            entity.HasOne("Esg.Api.Models.Unit", "Unit")
                .WithMany("Consumptions")
                .HasForeignKey("UnitId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Navigation("Unit");
        });

        modelBuilder.Entity("Esg.Api.Models.Unit", entity =>
        {
            entity.Navigation("Alerts");
            entity.Navigation("Consumptions");
            entity.Navigation("Devices");
        });
    }
}
