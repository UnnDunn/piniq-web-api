using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogSnapshotManufacturersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manufacturers",
                table: "CatalogSnapshots");

            migrationBuilder.CreateTable(
                name: "CatalogSnapshotManufacturers",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    CatalogSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogSnapshotManufacturers", x => new { x.CatalogSnapshotId, x.ManufacturerId });
                    table.ForeignKey(
                        name: "FK_CatalogSnapshotManufacturers_CatalogSnapshots_CatalogSnapshotId",
                        column: x => x.CatalogSnapshotId,
                        principalTable: "CatalogSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogSnapshotManufacturers");

            migrationBuilder.AddColumn<string>(
                name: "Manufacturers",
                table: "CatalogSnapshots",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
