using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Articles.Storage.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class OutboxWithActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpanId",
                table: "OutboxMessages",
                type: "character(16)",
                fixedLength: true,
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraceId",
                table: "OutboxMessages",
                type: "character(32)",
                fixedLength: true,
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpanId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "TraceId",
                table: "OutboxMessages");
        }
    }
}
