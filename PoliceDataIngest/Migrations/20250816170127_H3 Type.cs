using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDataIngest.Migrations
{
    /// <inheritdoc />
    public partial class H3Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "H3",
                table: "CrimeAreas",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "h3index");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "H3",
                table: "CrimeAreas",
                type: "h3index",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
