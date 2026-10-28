using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class NewDisableClubCourt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCourtActive",
                table: "Courts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsClubActive",
                table: "Clubs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCourtActive",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "IsClubActive",
                table: "Clubs");
        }
    }
}
