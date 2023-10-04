using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    internal class TileContext : DbContext {
        public DbSet<Tile> Tiles { get; set; }

        public TileContext() : base("DungeonContext") { }
    }
}
