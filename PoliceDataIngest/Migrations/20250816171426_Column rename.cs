using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class Columnrename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "H3",
                table: "CrimeAreas",
                newName: "h3");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "CrimeAreas",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Burglary",
                table: "CrimeAreas",
                newName: "burglary");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CrimeAreas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WeaponCrime",
                table: "CrimeAreas",
                newName: "weapon_crime");

            migrationBuilder.RenameColumn(
                name: "PersonalTheft",
                table: "CrimeAreas",
                newName: "personal_theft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "h3",
                table: "CrimeAreas",
                newName: "H3");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "CrimeAreas",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "burglary",
                table: "CrimeAreas",
                newName: "Burglary");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CrimeAreas",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weapon_crime",
                table: "CrimeAreas",
                newName: "WeaponCrime");

            migrationBuilder.RenameColumn(
                name: "personal_theft",
                table: "CrimeAreas",
                newName: "PersonalTheft");
        }
    }
}
