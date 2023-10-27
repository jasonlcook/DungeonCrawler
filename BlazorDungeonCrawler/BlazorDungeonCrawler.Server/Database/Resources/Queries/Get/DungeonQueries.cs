using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Queries.Get {
    public class DungeonQueries {
        protected readonly DungeonDbContext _dbContext;

        public DungeonQueries(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<Dungeon?> Get(Guid dungeonId) {
            try {
                return await _dbContext.Dungeons.Include("Adventurer").Include("Floors").Include("Floors.Tiles").Include("Floors.Tiles.Monsters").Include("Messages").Include("Messages.Children").Include("Messages.Children.Children").FirstAsync(d => d.Id == dungeonId);
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to retrieve the Dungeon.");
            }
        }
    }
}
