using Microsoft.EntityFrameworkCore.Migrations;

namespace K8sBackendShared.Data.Migrations
{
    public partial class AddJobEntityErrorsField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Errors",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Errors",
                table: "Jobs");
        }
    }
}
