using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Delete {
    public class MonsterDelete {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public MonsterDelete(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Delete(Guid mmonsterId) {
            try {
                Monster mmonster = _dbContext.Monsters.Where(m => m.Id == mmonsterId).First();

                _dbContext.Monsters.Remove(mmonster);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw new Exception("Database error while attempting to delete a Monster.");
            }
        }
    }
}
