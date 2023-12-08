using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class FloorUpdate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public FloorUpdate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Update(Floor floor) {
            try {
                _dbContext.Floors.Update(floor);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Floor updated");
            } catch (Exception ex) {
                _logger.LogError($"Floor Update Error: {ex.Message}");
                throw new Exception("Database error while attempting to update the Floor.");
            }
        }
    }
}
