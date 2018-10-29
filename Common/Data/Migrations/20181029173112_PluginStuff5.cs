using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepositoryUrl",
                table: "Templates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepositoryUrl",
                table: "Plugins",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepositoryUrl",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "RepositoryUrl",
                table: "Plugins");
        }
    }
}
