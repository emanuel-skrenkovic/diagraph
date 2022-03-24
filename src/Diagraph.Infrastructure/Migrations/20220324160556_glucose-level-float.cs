using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class glucoselevelfloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "level",
                table: "glucose_measurement",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "level",
                table: "glucose_measurement",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
