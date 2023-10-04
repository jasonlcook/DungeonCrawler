using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    internal class LevelContext : DbContext {
        public DbSet<Level> Levels { get; set; }
    }
}
