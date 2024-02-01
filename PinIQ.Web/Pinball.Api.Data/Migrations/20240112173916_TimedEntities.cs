using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class TimedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "OpdbChangelogs",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OpdbChangelogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "OpdbChangelogs",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "sysdatetimeoffset()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Updated",
                table: "OpdbChangelogs",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "sysdatetimeoffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Updated",
                table: "CatalogSnapshots",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "sysdatetimeoffset()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Published",
                table: "CatalogSnapshots",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Imported",
                table: "CatalogSnapshots",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "CatalogSnapshots",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "sysdatetimeoffset()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "OpdbChangelogs");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "OpdbChangelogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "OpdbChangelogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OpdbChangelogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                table: "CatalogSnapshots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "sysdatetimeoffset()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Published",
                table: "CatalogSnapshots",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Imported",
                table: "CatalogSnapshots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "CatalogSnapshots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "sysdatetimeoffset()");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CatalogSnapshots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
