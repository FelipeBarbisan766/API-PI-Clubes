using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomPlansAndPlanPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Plans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerAdminId",
                table: "Plans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanPricings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Component = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanPricings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PlanPricings",
                columns: new[] { "Id", "Component", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("22222222-0000-0000-0000-000000000001"), "Club", 30m, new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-0000-0000-0000-000000000002"), "Court", 10m, new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"),
                columns: new[] { "IsCustom", "OwnerAdminId" },
                values: new object[] { false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_OwnerAdminId",
                table: "Plans",
                column: "OwnerAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPricings_Component",
                table: "PlanPricings",
                column: "Component",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Admins_OwnerAdminId",
                table: "Plans",
                column: "OwnerAdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Admins_OwnerAdminId",
                table: "Plans");

            migrationBuilder.DropTable(
                name: "PlanPricings");

            migrationBuilder.DropIndex(
                name: "IX_Plans_OwnerAdminId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "OwnerAdminId",
                table: "Plans");
        }
    }
}
