using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Modules.GlucoseData.Migrations
{
    public partial class measurement_taken_at_timestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "taken_at",
                table: "glucose_measurement",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "taken_at",
                table: "glucose_measurement",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }
    }
}
