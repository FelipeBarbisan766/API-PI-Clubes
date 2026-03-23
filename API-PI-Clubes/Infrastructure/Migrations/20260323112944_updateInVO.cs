using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_PI_Clubes.Migrations
{
    /// <inheritdoc />
    public partial class updateInVO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubAdmins",
                table: "ClubAdmins");

            migrationBuilder.DropIndex(
                name: "IX_ClubAdmins_ClubId",
                table: "ClubAdmins");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Complement",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Neighborhood",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Number",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_ZipCode",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubAdmins",
                table: "ClubAdmins",
                columns: new[] { "ClubId", "AdminId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubAdmins",
                table: "ClubAdmins");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_Complement",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_Neighborhood",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_Number",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                table: "Clubs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubAdmins",
                table: "ClubAdmins",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClubAdmins_ClubId",
                table: "ClubAdmins",
                column: "ClubId");
        }
    }
}
