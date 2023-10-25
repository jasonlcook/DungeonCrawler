using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    /// <inheritdoc />
    public partial class DBSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adventurers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienceLevel = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    NextExperienceLevelCost = table.Column<int>(type: "int", nullable: false),
                    HealthBase = table.Column<int>(type: "int", nullable: false),
                    HealthInitial = table.Column<int>(type: "int", nullable: false),
                    AuraPotion = table.Column<int>(type: "int", nullable: false),
                    AuraPotionDuration = table.Column<int>(type: "int", nullable: false),
                    DamageBase = table.Column<int>(type: "int", nullable: false),
                    DamagePotion = table.Column<int>(type: "int", nullable: false),
                    DamagePotionDuration = table.Column<int>(type: "int", nullable: false),
                    ProtectionBase = table.Column<int>(type: "int", nullable: false),
                    ShieldPotion = table.Column<int>(type: "int", nullable: false),
                    ShieldPotionDuration = table.Column<int>(type: "int", nullable: false),
                    Weapon = table.Column<int>(type: "int", nullable: false),
                    ArmourHelmet = table.Column<int>(type: "int", nullable: false),
                    ArmourBreastplate = table.Column<int>(type: "int", nullable: false),
                    ArmourGauntlet = table.Column<int>(type: "int", nullable: false),
                    ArmourGreave = table.Column<int>(type: "int", nullable: false),
                    ArmourBoots = table.Column<int>(type: "int", nullable: false),
                    IsAlive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventurers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdventurerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    ApiVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MacGuffinFound = table.Column<bool>(type: "bit", nullable: false),
                    StairsDiscovered = table.Column<bool>(type: "bit", nullable: false),
                    InCombat = table.Column<bool>(type: "bit", nullable: false),
                    CombatTile = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CombatInitiated = table.Column<bool>(type: "bit", nullable: false),
                    GameOver = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dungeons_Adventurers_AdventurerId",
                        column: x => x.AdventurerId,
                        principalTable: "Adventurers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    Rows = table.Column<int>(type: "int", nullable: false),
                    Columns = table.Column<int>(type: "int", nullable: false),
                    DungeonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Floors_Dungeons_DungeonId",
                        column: x => x.DungeonId,
                        principalTable: "Dungeons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Datestamp = table.Column<double>(type: "float", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SafeDice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DangerDice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DungeonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Dungeons_DungeonId",
                        column: x => x.DungeonId,
                        principalTable: "Dungeons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Current = table.Column<bool>(type: "bit", nullable: false),
                    Hidden = table.Column<bool>(type: "bit", nullable: false),
                    Selectable = table.Column<bool>(type: "bit", nullable: false),
                    FightWon = table.Column<bool>(type: "bit", nullable: false),
                    FloorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tiles_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    Damage = table.Column<int>(type: "int", nullable: false),
                    Protection = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    ClientX = table.Column<int>(type: "int", nullable: false),
                    ClientY = table.Column<int>(type: "int", nullable: false),
                    TileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monsters_Tiles_TileId",
                        column: x => x.TileId,
                        principalTable: "Tiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dungeons_AdventurerId",
                table: "Dungeons",
                column: "AdventurerId");

            migrationBuilder.CreateIndex(
                name: "IX_Floors_DungeonId",
                table: "Floors",
                column: "DungeonId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DungeonId",
                table: "Messages",
                column: "DungeonId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageId",
                table: "Messages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_TileId",
                table: "Monsters",
                column: "TileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tiles_FloorId",
                table: "Tiles",
                column: "FloorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "Tiles");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropTable(
                name: "Dungeons");

            migrationBuilder.DropTable(
                name: "Adventurers");
        }
    }
}
