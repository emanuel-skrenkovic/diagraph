using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class userdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "import",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "glucose_measurement",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "event",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_tag_user_id",
                table: "tag",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_import_user_id",
                table: "import",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_glucose_measurement_user_id",
                table: "glucose_measurement",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_id",
                table: "event",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_user_user_id",
                table: "event",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_glucose_measurement_user_user_id",
                table: "glucose_measurement",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_import_user_user_id",
                table: "import",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tag_user_user_id",
                table: "tag",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_user_user_id",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_glucose_measurement_user_user_id",
                table: "glucose_measurement");

            migrationBuilder.DropForeignKey(
                name: "FK_import_user_user_id",
                table: "import");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_user_user_id",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "IX_tag_user_id",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "IX_import_user_id",
                table: "import");

            migrationBuilder.DropIndex(
                name: "IX_glucose_measurement_user_id",
                table: "glucose_measurement");

            migrationBuilder.DropIndex(
                name: "IX_event_user_id",
                table: "event");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "import");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "glucose_measurement");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "event");
        }
    }
}
