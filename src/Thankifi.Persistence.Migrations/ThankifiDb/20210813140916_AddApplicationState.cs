using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Thankifi.Persistence.Migrations.ThankifiDb;

public partial class AddApplicationState : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApplicationState",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("24c372ce-a7c2-4895-8241-3d8d432f61b7")),
                DatasetVersion = table.Column<int>(type: "integer", nullable: true),
                LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationState", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ApplicationState");
    }
}