using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCast.Identity.Api.Database.Migrations
{
    public partial class Rename_User_FirstName_To_GivenName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                schema: "dbo",
                table: "Users",
                newName: "GivenName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GivenName",
                schema: "dbo",
                table: "Users",
                newName: "FirstName");
        }
    }
}
