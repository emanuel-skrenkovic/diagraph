using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Modules.Events.Migrations
{
    public partial class importtemplateuseridname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "import_template",
                newName: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "import_template",
                newName: "UserId");
        }
    }
}
