using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    public class DungeonContext : DbContext {
        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Monster> Monsters { get; set; }

        public DungeonContext() : base("DungeonContext") { }
    }
}
