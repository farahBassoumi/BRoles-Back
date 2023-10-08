using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRoles.Migrations.OperationsDB
{
    public partial class op1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_userOperations",
                table: "userOperations");

            migrationBuilder.RenameTable(
                name: "userOperations",
                newName: "UserOperations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserOperations",
                table: "UserOperations",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserOperations",
                table: "UserOperations");

            migrationBuilder.RenameTable(
                name: "UserOperations",
                newName: "userOperations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userOperations",
                table: "userOperations",
                column: "Id");
        }
    }
}
