using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class newmigrationprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerHour",
                table: "Courts");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Schedules",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Schedules");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                table: "Courts",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
