using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRoles.Migrations
{
    public partial class desactivated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Desactivated",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desactivated",
                table: "users");
        }
    }
}
