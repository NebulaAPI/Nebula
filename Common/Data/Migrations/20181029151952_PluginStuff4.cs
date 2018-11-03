using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PluginDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    VersionPattern = table.Column<string>(nullable: true),
                    PluginId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginDependency_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginDependency_PluginId",
                table: "PluginDependency",
                column: "PluginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginDependency");
        }
    }
}
