using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class AdventurerUpdate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public AdventurerUpdate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Update(Adventurer adventurer) {
            try {
                _dbContext.Adventurers.Update(adventurer);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw new Exception("Database error while attempting to update the Adventurer.");
            }
        }
    }
}
