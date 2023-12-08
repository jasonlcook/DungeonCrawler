using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCurrentToFloor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Floors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Floors");
        }
    }
}
