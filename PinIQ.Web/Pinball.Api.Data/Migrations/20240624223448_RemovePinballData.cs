using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePinballData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PinballMachineFeatureMapping");

            migrationBuilder.DropTable(
                name: "PinballMachineKeywordMapping");

            migrationBuilder.DropTable(
                name: "PinballFeatures");

            migrationBuilder.DropTable(
                name: "PinballKeywords");

            migrationBuilder.DropTable(
                name: "PinballMachines");

            migrationBuilder.DropTable(
                name: "PinballMachineGroups");

            migrationBuilder.DropTable(
                name: "PinballManufacturers");

            migrationBuilder.DropTable(
                name: "PinballTypes");

            migrationBuilder.AlterColumn<string>(
                name: "OpdbId",
                table: "OpdbChangelogs",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NewOpdbId",
                table: "OpdbChangelogs",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogId",
                table: "OpdbChangelogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "OpdbChangelogs");

            migrationBuilder.AlterColumn<string>(
                name: "OpdbId",
                table: "OpdbChangelogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "NewOpdbId",
                table: "OpdbChangelogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PinballFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballKeywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballKeywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballManufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballManufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS"),
                    MachineGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true, collation: "SQL_Latin1_General_CP1_CS_AS"),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpdbId = table.Column<int>(type: "int", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerCount = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballMachineGroups_MachineGroupId",
                        column: x => x.MachineGroupId,
                        principalTable: "PinballMachineGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballManufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "PinballManufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PinballTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineFeatureMapping",
                columns: table => new
                {
                    FeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineId = table.Column<string>(type: "nvarchar(450)", nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineFeatureMapping", x => new { x.FeatureId, x.MachineId });
                    table.ForeignKey(
                        name: "FK_PinballMachineFeatureMapping_PinballFeatures_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "PinballFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PinballMachineFeatureMapping_PinballMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "PinballMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineKeywordMapping",
                columns: table => new
                {
                    KeywordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineId = table.Column<string>(type: "nvarchar(450)", nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineKeywordMapping", x => new { x.KeywordId, x.MachineId });
                    table.ForeignKey(
                        name: "FK_PinballMachineKeywordMapping_PinballKeywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "PinballKeywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PinballMachineKeywordMapping_PinballMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "PinballMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PinballTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "dmd", "Dot-Matrix Display" },
                    { "em", "Electro-Mechanical" },
                    { "me", "Mechanical" },
                    { "ss", "Solid-State" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachineFeatureMapping_MachineId",
                table: "PinballMachineFeatureMapping",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachineKeywordMapping_KeywordId",
                table: "PinballMachineKeywordMapping",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachineKeywordMapping_MachineId",
                table: "PinballMachineKeywordMapping",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_MachineGroupId",
                table: "PinballMachines",
                column: "MachineGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_ManufacturerId",
                table: "PinballMachines",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_TypeId",
                table: "PinballMachines",
                column: "TypeId");
        }
    }
}
