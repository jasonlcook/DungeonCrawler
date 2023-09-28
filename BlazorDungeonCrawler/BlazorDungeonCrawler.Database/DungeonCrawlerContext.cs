using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database {
    public class DungeonCrawlerContext : DbContext {
        public DungeonCrawlerContext() : base(){
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Tile> Tiles { get; set; }

        public DbSet<MonsterType> MonsterType { get; set; }
    }
}
