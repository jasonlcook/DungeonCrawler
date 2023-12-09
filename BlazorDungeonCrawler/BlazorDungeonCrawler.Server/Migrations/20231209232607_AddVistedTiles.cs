using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVistedTiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visited",
                table: "Tiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visited",
                table: "Tiles");
        }
    }
}
