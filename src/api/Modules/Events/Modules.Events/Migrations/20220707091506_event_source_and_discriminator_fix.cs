using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diagraph.Modules.Events.Migrations
{
    public partial class event_source_and_discriminator_fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "source",
                table: "event",
                type: "text",
                nullable: false);

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "event",
                newName: "discriminator");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "source",
                table: "event");
            
            migrationBuilder.RenameColumn(
                name: "discriminator",
                table: "event",
                newName: "Discriminator");
        }
    }
}
