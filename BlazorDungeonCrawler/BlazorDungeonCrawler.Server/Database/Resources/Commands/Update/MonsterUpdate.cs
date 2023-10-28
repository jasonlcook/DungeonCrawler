using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class MonsterUpdate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public MonsterUpdate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Update(Monster monster) {
            try {
                _dbContext.Monsters.Update(monster);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Monster updated");
            } catch (Exception ex) {
                _logger.LogError($"MonsterUpdate Update Error: {ex.Message}");
                throw new Exception("Database error while attempting to update a Monster.");
            }
        }
    }
}
