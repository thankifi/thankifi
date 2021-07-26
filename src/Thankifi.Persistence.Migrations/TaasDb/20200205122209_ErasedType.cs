using Microsoft.EntityFrameworkCore.Migrations;

namespace Thankifi.Persistence.Migrations.Migrations
{
    public partial class ErasedType : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Gratitudes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Gratitudes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
