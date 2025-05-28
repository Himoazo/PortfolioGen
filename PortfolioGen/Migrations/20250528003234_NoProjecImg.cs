using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioGen.Migrations
{
    /// <inheritdoc />
    public partial class NoProjecImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Projects",
                type: "TEXT",
                nullable: true);
        }
    }
}
