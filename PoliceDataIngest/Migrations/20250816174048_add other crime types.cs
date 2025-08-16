using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class addothercrimetypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "bicycle_theft",
                table: "crime_areas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "damage",
                table: "crime_areas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "robbery",
                table: "crime_areas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "shoplifting",
                table: "crime_areas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "violent",
                table: "crime_areas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bicycle_theft",
                table: "crime_areas");

            migrationBuilder.DropColumn(
                name: "damage",
                table: "crime_areas");

            migrationBuilder.DropColumn(
                name: "robbery",
                table: "crime_areas");

            migrationBuilder.DropColumn(
                name: "shoplifting",
                table: "crime_areas");

            migrationBuilder.DropColumn(
                name: "violent",
                table: "crime_areas");
        }
    }
}
