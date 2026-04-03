using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class useremail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailVerification_IsConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerification_VerifiedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerification_IsConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailVerification_VerifiedAt",
                table: "Users");
        }
    }
}
