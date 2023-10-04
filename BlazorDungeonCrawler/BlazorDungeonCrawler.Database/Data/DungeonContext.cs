using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    public class DungeonContext : DbContext {
        public DbSet<Dungeon> Dungeons { get; set; }

        public DungeonContext() : base("DungeonContext") { }
    }
}
