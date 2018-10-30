using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class TemplateStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LanguagePluginId",
                table: "Templates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TemplateLanguagePlugin",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateLanguagePlugin", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Templates_LanguagePluginId",
                table: "Templates",
                column: "LanguagePluginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_TemplateLanguagePlugin_LanguagePluginId",
                table: "Templates",
                column: "LanguagePluginId",
                principalTable: "TemplateLanguagePlugin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_TemplateLanguagePlugin_LanguagePluginId",
                table: "Templates");

            migrationBuilder.DropTable(
                name: "TemplateLanguagePlugin");

            migrationBuilder.DropIndex(
                name: "IX_Templates_LanguagePluginId",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "LanguagePluginId",
                table: "Templates");
        }
    }
}
