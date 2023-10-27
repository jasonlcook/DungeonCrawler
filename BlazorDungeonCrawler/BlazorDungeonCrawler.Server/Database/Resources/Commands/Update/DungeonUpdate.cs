using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public  class DungeonUpdate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public DungeonUpdate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Update(Dungeon dungeon) {
            try {
                _dbContext.Dungeons.Update(dungeon);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw new Exception("Database error while attempting to update the Dungeon.");
            }
        }
    }
}
