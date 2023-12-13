using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDexterity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dexterity",
                table: "Monsters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DexterityBase",
                table: "Adventurers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dexterity",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "DexterityBase",
                table: "Adventurers");
        }
    }
}
