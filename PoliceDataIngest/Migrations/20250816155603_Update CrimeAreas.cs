using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCrimeAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Burglary",
                table: "CrimeAreas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CrimeAreas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "H3",
                table: "CrimeAreas",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "PersonalTheft",
                table: "CrimeAreas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WeaponCrime",
                table: "CrimeAreas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Burglary",
                table: "CrimeAreas");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "CrimeAreas");

            migrationBuilder.DropColumn(
                name: "H3",
                table: "CrimeAreas");

            migrationBuilder.DropColumn(
                name: "PersonalTheft",
                table: "CrimeAreas");

            migrationBuilder.DropColumn(
                name: "WeaponCrime",
                table: "CrimeAreas");
        }
    }
}
