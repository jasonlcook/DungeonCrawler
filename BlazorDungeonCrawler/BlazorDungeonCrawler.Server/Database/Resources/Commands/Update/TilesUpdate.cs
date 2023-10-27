using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class TilesUpdate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public TilesUpdate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task Update(List<Tile> tiles) {
            try {
                foreach (var tile in tiles) {
                    _dbContext.Tiles.Update(tile);
                }

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw new Exception("Database error while attempting to update a Tile.");
            }
        }
    }
}
