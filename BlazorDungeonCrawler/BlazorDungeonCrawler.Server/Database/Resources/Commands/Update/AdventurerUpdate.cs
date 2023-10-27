using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class AdventurerUpdate  {
        protected readonly DungeonDbContext _dbContext;

        public AdventurerUpdate(DungeonDbContext dbContext) {
            _dbContext = (DungeonDbContext)dbContext;
        }

        public async Task Update(Adventurer adventurer) {
            try {
                _dbContext.Adventurers.Update(adventurer);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to update the Adventurer.");
            }
        }
    }
}
