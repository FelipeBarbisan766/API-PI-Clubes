using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Flags",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Flags",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Flags",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("22222222-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Reservou a quadra e não compareceu, sem avisar.", true, "No-show", null },
                    { new Guid("22222222-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chegou muito depois do horário reservado, prejudicando o uso da quadra.", true, "Atraso excessivo", null },
                    { new Guid("22222222-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cancelou a reserva sem antecedência, deixando o horário vazio.", true, "Cancelamento em cima da hora", null },
                    { new Guid("22222222-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Causou danos à quadra ou aos equipamentos do clube.", true, "Danos à quadra", null },
                    { new Guid("22222222-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Conduta agressiva ou desrespeitosa com outros jogadores ou funcionários.", true, "Comportamento inadequado", null },
                    { new Guid("22222222-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Utilizou a quadra e não realizou o pagamento.", true, "Falta de pagamento", null }
                });

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"),
                column: "QuantCourt",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Flags_Name",
                table: "Flags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flags_Name",
                table: "Flags");

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Flags",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000006"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Flags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Flags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"),
                column: "QuantCourt",
                value: 0);
        }
    }
}
