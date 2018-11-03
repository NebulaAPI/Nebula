using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateDependency_Templates_TemplateId",
                table: "TemplateDependency");

            migrationBuilder.DropTable(
                name: "PluginDependency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateDependency",
                table: "TemplateDependency");

            migrationBuilder.RenameTable(
                name: "TemplateDependency",
                newName: "BaseDependency");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateDependency_TemplateId",
                table: "BaseDependency",
                newName: "IX_BaseDependency_TemplateId");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "TemplateVersion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "TemplateVersion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseDependency",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PluginVersionId",
                table: "BaseDependency",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateVersionId",
                table: "BaseDependency",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseDependency",
                table: "BaseDependency",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDependency_PluginVersionId",
                table: "BaseDependency",
                column: "PluginVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDependency_TemplateVersionId",
                table: "BaseDependency",
                column: "TemplateVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseDependency_PluginVersion_PluginVersionId",
                table: "BaseDependency",
                column: "PluginVersionId",
                principalTable: "PluginVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseDependency_Templates_TemplateId",
                table: "BaseDependency",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseDependency_TemplateVersion_TemplateVersionId",
                table: "BaseDependency",
                column: "TemplateVersionId",
                principalTable: "TemplateVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseDependency_PluginVersion_PluginVersionId",
                table: "BaseDependency");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseDependency_Templates_TemplateId",
                table: "BaseDependency");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseDependency_TemplateVersion_TemplateVersionId",
                table: "BaseDependency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseDependency",
                table: "BaseDependency");

            migrationBuilder.DropIndex(
                name: "IX_BaseDependency_PluginVersionId",
                table: "BaseDependency");

            migrationBuilder.DropIndex(
                name: "IX_BaseDependency_TemplateVersionId",
                table: "BaseDependency");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "TemplateVersion");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "TemplateVersion");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseDependency");

            migrationBuilder.DropColumn(
                name: "PluginVersionId",
                table: "BaseDependency");

            migrationBuilder.DropColumn(
                name: "TemplateVersionId",
                table: "BaseDependency");

            migrationBuilder.RenameTable(
                name: "BaseDependency",
                newName: "TemplateDependency");

            migrationBuilder.RenameIndex(
                name: "IX_BaseDependency_TemplateId",
                table: "TemplateDependency",
                newName: "IX_TemplateDependency_TemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateDependency",
                table: "TemplateDependency",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PluginDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PluginVersionId = table.Column<Guid>(nullable: true),
                    VersionPattern = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginDependency_PluginVersion_PluginVersionId",
                        column: x => x.PluginVersionId,
                        principalTable: "PluginVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginDependency_PluginVersionId",
                table: "PluginDependency",
                column: "PluginVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateDependency_Templates_TemplateId",
                table: "TemplateDependency",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
