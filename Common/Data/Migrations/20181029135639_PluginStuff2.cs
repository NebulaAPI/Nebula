using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateVersion_Plugins_PluginId",
                table: "TemplateVersion");

            migrationBuilder.DropIndex(
                name: "IX_TemplateVersion_PluginId",
                table: "TemplateVersion");

            migrationBuilder.DropColumn(
                name: "PluginId",
                table: "TemplateVersion");

            migrationBuilder.CreateTable(
                name: "PluginVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    CommitSha = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    PluginId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginVersion_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersion_PluginId",
                table: "PluginVersion",
                column: "PluginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "PluginId",
                table: "TemplateVersion",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateVersion_PluginId",
                table: "TemplateVersion",
                column: "PluginId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateVersion_Plugins_PluginId",
                table: "TemplateVersion",
                column: "PluginId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
