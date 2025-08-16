using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class renamecrimeareastable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CrimeAreas",
                table: "CrimeAreas");

            migrationBuilder.RenameTable(
                name: "CrimeAreas",
                newName: "crime_areas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas");

            migrationBuilder.RenameTable(
                name: "crime_areas",
                newName: "CrimeAreas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrimeAreas",
                table: "CrimeAreas",
                column: "id");
        }
    }
}
