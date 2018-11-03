using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class PluginStuff3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugins_User_AuthorId",
                table: "Plugins");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_User_AuthorId",
                table: "Templates");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Templates",
                newName: "UploadedById");

            migrationBuilder.RenameIndex(
                name: "IX_Templates_AuthorId",
                table: "Templates",
                newName: "IX_Templates_UploadedById");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Plugins",
                newName: "UploadedById");

            migrationBuilder.RenameIndex(
                name: "IX_Plugins_AuthorId",
                table: "Plugins",
                newName: "IX_Plugins_UploadedById");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Templates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Plugins",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Plugins_User_UploadedById",
                table: "Plugins",
                column: "UploadedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_User_UploadedById",
                table: "Templates",
                column: "UploadedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugins_User_UploadedById",
                table: "Plugins");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_User_UploadedById",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Plugins");

            migrationBuilder.RenameColumn(
                name: "UploadedById",
                table: "Templates",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Templates_UploadedById",
                table: "Templates",
                newName: "IX_Templates_AuthorId");

            migrationBuilder.RenameColumn(
                name: "UploadedById",
                table: "Plugins",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Plugins_UploadedById",
                table: "Plugins",
                newName: "IX_Plugins_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plugins_User_AuthorId",
                table: "Plugins",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_User_AuthorId",
                table: "Templates",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
