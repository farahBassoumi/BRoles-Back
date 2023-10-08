using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRoles.Migrations
{
    public partial class addrole2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departement",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "users",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "users",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.AddColumn<string>(
                name: "Departement",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "phone",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
