using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database {
    public class DungeonDbContext : DbContext {

        public DungeonDbContext(DbContextOptions<DungeonDbContext> options) : base(options) { }

        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<AccessTokenLog> AccessTokenLogs { get; set; }

        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Monster> Monsters { get; set; }
    }
}