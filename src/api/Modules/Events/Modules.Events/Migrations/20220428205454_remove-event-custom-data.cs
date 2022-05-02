using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Modules.Events.Migrations
{
    public partial class removeeventcustomdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "custom_data",
                table: "event");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "custom_data",
                table: "event",
                type: "jsonb",
                nullable: true);
        }
    }
}
