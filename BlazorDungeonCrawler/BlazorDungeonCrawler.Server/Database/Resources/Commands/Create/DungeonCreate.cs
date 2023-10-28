using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class DungeonCreate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public DungeonCreate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Create(Dungeon dungeon) {
            try {
                _dbContext.Dungeons.Add(dungeon);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New Dungeon created");
            } catch (Exception ex) {
                _logger.LogError($"DungeonCreate Create Error: {ex.Message}");
                throw new Exception("Database error while attempting to create a new Dungeon.");
            }
        }
    }
}
