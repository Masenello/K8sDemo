using Microsoft.EntityFrameworkCore.Migrations;

namespace K8sBackendShared.Data.Migrations
{
    public partial class AddWorkerIdToJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkerId",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Jobs");
        }
    }
}
