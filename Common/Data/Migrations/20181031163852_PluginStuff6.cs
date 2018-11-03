using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PluginDependency_Plugins_PluginId",
                table: "PluginDependency");

            migrationBuilder.RenameColumn(
                name: "PluginId",
                table: "PluginDependency",
                newName: "PluginVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_PluginDependency_PluginId",
                table: "PluginDependency",
                newName: "IX_PluginDependency_PluginVersionId");

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "PluginVersion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PluginDependency_PluginVersion_PluginVersionId",
                table: "PluginDependency",
                column: "PluginVersionId",
                principalTable: "PluginVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PluginDependency_PluginVersion_PluginVersionId",
                table: "PluginDependency");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "PluginVersion");

            migrationBuilder.RenameColumn(
                name: "PluginVersionId",
                table: "PluginDependency",
                newName: "PluginId");

            migrationBuilder.RenameIndex(
                name: "IX_PluginDependency_PluginVersionId",
                table: "PluginDependency",
                newName: "IX_PluginDependency_PluginId");

            migrationBuilder.AddForeignKey(
                name: "FK_PluginDependency_Plugins_PluginId",
                table: "PluginDependency",
                column: "PluginId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
