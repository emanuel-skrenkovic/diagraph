using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class glucosemeasurementtakenat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "taken_at",
                table: "glucose_measurement",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "taken_at",
                table: "glucose_measurement");
        }
    }
}
