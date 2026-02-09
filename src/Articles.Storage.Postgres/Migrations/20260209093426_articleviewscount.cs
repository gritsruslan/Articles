using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Articles.Storage.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class articleviewscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ViewsCount",
                table: "Articles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewsCount",
                table: "Articles");
        }
    }
}
