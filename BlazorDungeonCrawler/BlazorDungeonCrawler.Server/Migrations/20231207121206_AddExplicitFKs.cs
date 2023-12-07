using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Dungeons_DungeonId",
                table: "Messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "DungeonId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Dungeons_DungeonId",
                table: "Messages",
                column: "DungeonId",
                principalTable: "Dungeons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Dungeons_DungeonId",
                table: "Messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "DungeonId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Dungeons_DungeonId",
                table: "Messages",
                column: "DungeonId",
                principalTable: "Dungeons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
