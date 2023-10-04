using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    internal class MonstersContext : DbContext {
        public DbSet<Monster> Monsters { get; set; }

        public MonstersContext() : base("DungeonContext") { }
    }
}
