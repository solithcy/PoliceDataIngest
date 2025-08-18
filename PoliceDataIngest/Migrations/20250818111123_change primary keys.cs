using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class changeprimarykeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas");

            migrationBuilder.DropColumn(
                name: "id",
                table: "crime_areas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas",
                columns: new[] { "date", "h3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "crime_areas",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_crime_areas",
                table: "crime_areas",
                column: "id");
        }
    }
}
