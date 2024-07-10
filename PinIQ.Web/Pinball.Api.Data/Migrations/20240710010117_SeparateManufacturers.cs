using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateManufacturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Manufacturers",
                table: "CatalogSnapshots",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manufacturers",
                table: "CatalogSnapshots");
        }
    }
}
