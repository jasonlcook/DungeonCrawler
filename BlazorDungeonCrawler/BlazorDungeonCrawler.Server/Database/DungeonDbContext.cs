using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database {
    public class DungeonDbContext : DbContext {
        public IConfiguration _dbContext { get; set; }

        public DungeonDbContext(IConfiguration _dbContext) {
            this._dbContext = _dbContext;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_dbContext.GetConnectionString("BlazorDungeonCrawler"));
        }

        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Monster> Monsters { get; set; }
    }
}