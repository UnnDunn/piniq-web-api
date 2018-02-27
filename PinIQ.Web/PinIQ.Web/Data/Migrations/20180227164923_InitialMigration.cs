using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Pinball.Web.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateTable(
                name: "PinballTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IpdbUrl = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PlayerCount = table.Column<short>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    SortName = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinballTableCatalog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JsonAbbreviationData = table.Column<string>(nullable: true),
                    JsonTableData = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: true),
                    PublisherID = table.Column<string>(nullable: true),
                    RawAbbreviationData = table.Column<string>(nullable: true),
                    RawAbbreviationDataSize = table.Column<long>(nullable: false),
                    RawTableData = table.Column<string>(nullable: true),
                    RawTableDataSize = table.Column<long>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    UploaderID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinballTableCatalog", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PinballTableCatalog_AspNetUsers_PublisherID",
                        column: x => x.PublisherID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PinballTableCatalog_AspNetUsers_UploaderID",
                        column: x => x.UploaderID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Abbreviation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TableID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abbreviation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Abbreviation_PinballTable_TableID",
                        column: x => x.TableID,
                        principalTable: "PinballTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Abbreviation_TableID",
                table: "Abbreviation",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_PinballTableCatalog_PublisherID",
                table: "PinballTableCatalog",
                column: "PublisherID");

            migrationBuilder.CreateIndex(
                name: "IX_PinballTableCatalog_UploaderID",
                table: "PinballTableCatalog",
                column: "UploaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Abbreviation");

            migrationBuilder.DropTable(
                name: "PinballTableCatalog");

            migrationBuilder.DropTable(
                name: "PinballTable");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
