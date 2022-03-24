using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diagraph.Infrastructure.Migrations
{
    public partial class glucose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_insulin_application_meal_MealId1",
                table: "insulin_application");

            migrationBuilder.DropIndex(
                name: "IX_insulin_application_MealId1",
                table: "insulin_application");

            migrationBuilder.DropColumn(
                name: "MealId1",
                table: "insulin_application");

            migrationBuilder.CreateTable(
                name: "import",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hash = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_import", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "glucose_measurement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    level = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    import_id = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_glucose_measurement", x => x.id);
                    table.ForeignKey(
                        name: "FK_glucose_measurement_import_import_id",
                        column: x => x.import_id,
                        principalTable: "import",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_glucose_measurement_import_id",
                table: "glucose_measurement",
                column: "import_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "glucose_measurement");

            migrationBuilder.DropTable(
                name: "import");

            migrationBuilder.AddColumn<int>(
                name: "MealId1",
                table: "insulin_application",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_insulin_application_MealId1",
                table: "insulin_application",
                column: "MealId1");

            migrationBuilder.AddForeignKey(
                name: "FK_insulin_application_meal_MealId1",
                table: "insulin_application",
                column: "MealId1",
                principalTable: "meal",
                principalColumn: "id");
        }
    }
}
