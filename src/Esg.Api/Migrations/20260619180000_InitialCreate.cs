using System;
using Esg.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Esg.Api.Migrations;

[DbContext(typeof(EsgDbContext))]
[Migration("20260619180000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "app_users",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_app_users", x => x.Id));

        migrationBuilder.CreateTable(
            name: "units",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_units", x => x.Id));

        migrationBuilder.CreateTable(
            name: "consumption_alerts",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UnitId = table.Column<long>(type: "bigint", nullable: false),
                AlertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_consumption_alerts", x => x.Id);
                table.ForeignKey(
                    name: "FK_consumption_alerts_units_UnitId",
                    column: x => x.UnitId,
                    principalTable: "units",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "devices",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UnitId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                DeviceType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                RatedPowerW = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_devices", x => x.Id);
                table.ForeignKey(
                    name: "FK_devices_units_UnitId",
                    column: x => x.UnitId,
                    principalTable: "units",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "energy_consumptions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UnitId = table.Column<long>(type: "bigint", nullable: false),
                KwhConsumed = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                ReferenceMonth = table.Column<DateOnly>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_energy_consumptions", x => x.Id);
                table.ForeignKey(
                    name: "FK_energy_consumptions_units_UnitId",
                    column: x => x.UnitId,
                    principalTable: "units",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.InsertData(
            table: "app_users",
            columns: new[] { "Id", "PasswordHash", "Role", "UserName" },
            columnTypes: new[] { "bigint", "nvarchar(128)", "nvarchar(20)", "nvarchar(80)" },
            values: new object[,]
            {
                { 1L, "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", "ADMIN", "admin" },
                { 2L, "ec6e1c25258002eb1c67d15c7f45da7945fa4c58778fd7d88faa5e53e3b4698d", "OPERATOR", "operator" }
            });

        migrationBuilder.InsertData(
            table: "units",
            columns: new[] { "Id", "City", "Name" },
            columnTypes: new[] { "bigint", "nvarchar(80)", "nvarchar(120)" },
            values: new object[,]
            {
                { 1L, "Sao Paulo", "Unidade Paulista" },
                { 2L, "Sao Paulo", "Unidade Vila Olimpia" },
                { 3L, "Barueri", "Unidade Alphaville" }
            });

        migrationBuilder.InsertData(
            table: "consumption_alerts",
            columns: new[] { "Id", "AlertDate", "Message", "Status", "UnitId" },
            columnTypes: new[] { "bigint", "datetime2", "nvarchar(300)", "nvarchar(20)", "bigint" },
            values: new object[] { 1L, new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc), "Consumo mensal acima da meta definida.", "OPEN", 1L });

        migrationBuilder.InsertData(
            table: "devices",
            columns: new[] { "Id", "DeviceType", "LastActivityAt", "Name", "RatedPowerW", "Status", "UnitId" },
            columnTypes: new[] { "bigint", "nvarchar(80)", "datetime2", "nvarchar(120)", "decimal(12,2)", "nvarchar(20)", "bigint" },
            values: new object[,]
            {
                { 1L, "HVAC", new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc), "HVAC Bloco A", 18000m, "ONLINE", 1L },
                { 2L, "LIGHTING", new DateTime(2026, 5, 30, 22, 0, 0, DateTimeKind.Utc), "Iluminacao Terreo", 4200m, "STANDBY", 1L },
                { 3L, "IT", new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc), "Data Center", 26000m, "ONLINE", 2L }
            });

        migrationBuilder.InsertData(
            table: "energy_consumptions",
            columns: new[] { "Id", "KwhConsumed", "ReferenceMonth", "UnitId" },
            columnTypes: new[] { "bigint", "decimal(12,2)", "date", "bigint" },
            values: new object[,]
            {
                { 1L, 14820m, new DateOnly(2026, 4, 1), 1L },
                { 2L, 15140m, new DateOnly(2026, 5, 1), 1L },
                { 3L, 11210m, new DateOnly(2026, 5, 1), 2L },
                { 4L, 7840m, new DateOnly(2026, 5, 1), 3L }
            });

        migrationBuilder.CreateIndex(
            name: "IX_app_users_UserName",
            table: "app_users",
            column: "UserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_consumption_alerts_UnitId_Status_AlertDate",
            table: "consumption_alerts",
            columns: new[] { "UnitId", "Status", "AlertDate" });

        migrationBuilder.CreateIndex(
            name: "IX_devices_UnitId_Status",
            table: "devices",
            columns: new[] { "UnitId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_energy_consumptions_UnitId_ReferenceMonth",
            table: "energy_consumptions",
            columns: new[] { "UnitId", "ReferenceMonth" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "app_users");
        migrationBuilder.DropTable(name: "consumption_alerts");
        migrationBuilder.DropTable(name: "devices");
        migrationBuilder.DropTable(name: "energy_consumptions");
        migrationBuilder.DropTable(name: "units");
    }
}
