using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Modules.Identity.Migrations
{
    public partial class processmanagernaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "process",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "process",
                newName: "created_at_utc");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at_utc",
                table: "process",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at_utc",
                table: "process",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "process",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "process",
                newName: "CreatedAtUtc");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "process",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "process",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");
        }
    }
}
