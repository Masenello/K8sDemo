using Microsoft.EntityFrameworkCore.Migrations;

namespace k8sData.Data.Migrations
{
    public partial class AddTimeoutToJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeOutSeconds",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeOutSeconds",
                table: "Jobs");
        }
    }
}
