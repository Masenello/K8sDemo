using Microsoft.EntityFrameworkCore.Migrations;

namespace K8sData.Data.Migrations
{
    public partial class RemoveNotesOnTestJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
