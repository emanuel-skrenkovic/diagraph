using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class usersecuritystamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "security_stamp",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "security_stamp",
                table: "user");
        }
    }
}
