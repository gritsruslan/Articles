using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Articles.Storage.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class M : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_CreatorId",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Articles",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_CreatorId",
                table: "Articles",
                newName: "IX_Articles_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Articles",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                newName: "IX_Articles_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_CreatorId",
                table: "Articles",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
