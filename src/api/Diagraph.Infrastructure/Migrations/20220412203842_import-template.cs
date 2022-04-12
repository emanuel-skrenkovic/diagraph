using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class importtemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_tag_tag_tag_id",
                table: "event_tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_event_tag",
                table: "event_tag");

            migrationBuilder.DropIndex(
                name: "IX_event_tag_tag_id",
                table: "event_tag");

            migrationBuilder.DropColumn(
                name: "tag_id",
                table: "event_tag");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "event_tag",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "event",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_event_tag",
                table: "event_tag",
                columns: new[] { "event_id", "name" });

            migrationBuilder.CreateTable(
                name: "import_template",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_import_template", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "import_template");

            migrationBuilder.DropPrimaryKey(
                name: "PK_event_tag",
                table: "event_tag");

            migrationBuilder.DropColumn(
                name: "name",
                table: "event_tag");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "event");

            migrationBuilder.AddColumn<int>(
                name: "tag_id",
                table: "event_tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_event_tag",
                table: "event_tag",
                columns: new[] { "event_id", "tag_id" });

            migrationBuilder.CreateIndex(
                name: "IX_event_tag_tag_id",
                table: "event_tag",
                column: "tag_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_tag_tag_tag_id",
                table: "event_tag",
                column: "tag_id",
                principalTable: "tag",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
