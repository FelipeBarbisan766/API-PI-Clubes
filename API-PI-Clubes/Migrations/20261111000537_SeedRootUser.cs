using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class SeedRootUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "BirthDate", "CpfEncrypted", "CpfHash", "CreatedAt", "DeletedAt", "Description", "Email", "IsActive", "Name", "PasswordHash", "PhoneNumber", "Provider", "Role", "UpdatedAt", "EmailVerification_IsConfirmed", "EmailVerification_VerifiedAt" },
                values: new object[] { new Guid("99999999-0000-0000-0000-000000000001"), null, null, null, null, new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "fgatechnology@gmail.com", true, "Root", null, null, "google", 3, null, true, new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("99999999-0000-0000-0000-000000000001"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
