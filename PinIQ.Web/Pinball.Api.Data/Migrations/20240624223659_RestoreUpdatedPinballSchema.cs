using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestoreUpdatedPinballSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pinball");

            migrationBuilder.CreateTable(
                name: "PinballFeatures",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballKeywords",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballKeywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineGroups",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpdbId = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballManufacturers",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballManufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballTypes",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachines",
                schema: "pinball",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpdbId = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CommonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpdbId = table.Column<int>(type: "int", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    PlayerCount = table.Column<short>(type: "smallint", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    MachineGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballMachineGroups_MachineGroupId",
                        column: x => x.MachineGroupId,
                        principalSchema: "pinball",
                        principalTable: "PinballMachineGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballManufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalSchema: "pinball",
                        principalTable: "PinballManufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PinballMachines_PinballTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "pinball",
                        principalTable: "PinballTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineFeatureMapping",
                schema: "pinball",
                columns: table => new
                {
                    FeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineFeatureMapping", x => new { x.FeatureId, x.MachineId });
                    table.ForeignKey(
                        name: "FK_PinballMachineFeatureMapping_PinballFeatures_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "pinball",
                        principalTable: "PinballFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PinballMachineFeatureMapping_PinballMachines_MachineId",
                        column: x => x.MachineId,
                        principalSchema: "pinball",
                        principalTable: "PinballMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PinballMachineKeywordMapping",
                schema: "pinball",
                columns: table => new
                {
                    KeywordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballMachineKeywordMapping", x => new { x.KeywordId, x.MachineId });
                    table.ForeignKey(
                        name: "FK_PinballMachineKeywordMapping_PinballKeywords_KeywordId",
                        column: x => x.KeywordId,
                        principalSchema: "pinball",
                        principalTable: "PinballKeywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PinballMachineKeywordMapping_PinballMachines_MachineId",
                        column: x => x.MachineId,
                        principalSchema: "pinball",
                        principalTable: "PinballMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "pinball",
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
                schema: "pinball",
                table: "PinballMachineFeatureMapping",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachineKeywordMapping_KeywordId",
                schema: "pinball",
                table: "PinballMachineKeywordMapping",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachineKeywordMapping_MachineId",
                schema: "pinball",
                table: "PinballMachineKeywordMapping",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_MachineGroupId",
                schema: "pinball",
                table: "PinballMachines",
                column: "MachineGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_ManufacturerId",
                schema: "pinball",
                table: "PinballMachines",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_PinballMachines_TypeId",
                schema: "pinball",
                table: "PinballMachines",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PinballMachineFeatureMapping",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballMachineKeywordMapping",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballFeatures",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballKeywords",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballMachines",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballMachineGroups",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballManufacturers",
                schema: "pinball");

            migrationBuilder.DropTable(
                name: "PinballTypes",
                schema: "pinball");
        }
    }
}
