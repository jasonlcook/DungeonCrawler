using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database {
    public class DungeonCrawlerContext : DbContext {
        public DungeonCrawlerContext() {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Tile> Tiles { get; set; }
    }
}
