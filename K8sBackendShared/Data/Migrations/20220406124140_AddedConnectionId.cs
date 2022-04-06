using Microsoft.EntityFrameworkCore.Migrations;

namespace K8sBackendShared.Data.Migrations
{
    public partial class AddedConnectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "ConnectedApps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "ConnectedApps");
        }
    }
}
