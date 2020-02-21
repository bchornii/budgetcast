using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetCast.Dashboard.Api.Infrastructure.Migrations.IdentityMigrations
{
    public partial class UpdateUserProfileWithImageLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "ProfileImageLink", 
                table: "AspNetUsers", type: "NVARCHAR(MAX)", unicode: true, nullable: true);

            migrationBuilder.AddColumn<string>(name: "ProfileImageThumbnailLink",
                table: "AspNetUsers", type: "NVARCHAR(MAX)", unicode: true, nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ProfileImageLink", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "ProfileImageThumbnailLink", table: "AspNetUsers");
        }
    }
}
