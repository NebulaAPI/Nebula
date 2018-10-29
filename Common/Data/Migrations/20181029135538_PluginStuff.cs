using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PluginId",
                table: "TemplateVersion",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AuthorId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Published = table.Column<DateTime>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plugins_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateVersion_PluginId",
                table: "TemplateVersion",
                column: "PluginId");

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_AuthorId",
                table: "Plugins",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateVersion_Plugins_PluginId",
                table: "TemplateVersion",
                column: "PluginId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateVersion_Plugins_PluginId",
                table: "TemplateVersion");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropIndex(
                name: "IX_TemplateVersion_PluginId",
                table: "TemplateVersion");

            migrationBuilder.DropColumn(
                name: "PluginId",
                table: "TemplateVersion");
        }
    }
}
