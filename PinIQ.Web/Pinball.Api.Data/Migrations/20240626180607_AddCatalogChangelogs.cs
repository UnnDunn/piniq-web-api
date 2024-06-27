using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogChangelogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogChangelogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PinballMachines = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinballMachineGroups = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinballManufacturers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "sysdatetimeoffset()"),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "sysdatetimeoffset()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogChangelogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogChangelogs");
        }
    }
}
