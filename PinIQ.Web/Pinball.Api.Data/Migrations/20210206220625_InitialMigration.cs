using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pinball.Api.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Published = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MachineJsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineGroupJsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpdbChangelogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpdbId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewOpdbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpdbChangelogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    CommonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpdbId = table.Column<int>(type: "int", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    PlayerCount = table.Column<short>(type: "smallint", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MachineGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true, collation: "SQL_Latin1_General_CP1_CS_AS")
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
                    { "ss", "Solid-State" },
                    { "em", "Electro-Mechanical" },
                    { "me", "Mechanical" },
                    { "dmd", "Dot-Matrix Display" }
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogSnapshots");

            migrationBuilder.DropTable(
                name: "OpdbChangelogs");

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
        }
    }
}
