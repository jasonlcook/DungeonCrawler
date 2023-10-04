using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    public class AdventurerContext : DbContext {
        public DbSet<Adventurer> Adventurers { get; set; }
    }
}

//Add-Migration AdventurerInitMigration -ConfigurationTypeName BlazorDungeonCrawler.Database.Migrations.AdventurerConfiguration