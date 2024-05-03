using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OpdbCatalogSnapshotRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeywordCount",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MachineCount",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MachineGroupCount",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MachineGroups",
                table: "CatalogSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Machines",
                table: "CatalogSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerCount",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NewestMachine",
                table: "CatalogSnapshots",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeywordCount",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "MachineCount",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "MachineGroupCount",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "MachineGroups",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "Machines",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "ManufacturerCount",
                table: "CatalogSnapshots");

            migrationBuilder.DropColumn(
                name: "NewestMachine",
                table: "CatalogSnapshots");
        }
    }
}
