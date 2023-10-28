using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class FloorCreate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public FloorCreate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Create(Guid dungeonId, Floor floor) {
            try {
                Dungeon? attachedDungeon = _dbContext.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();

                if (attachedDungeon != null) {
                    attachedDungeon.Floors = new();
                    attachedDungeon.Floors.Add(floor);

                    _dbContext.Floors.Add(attachedDungeon.Floors.First());
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New Floor created");
            } catch (Exception ex) {
                _logger.LogError($"FloorCreate Create Error: {ex.Message}");
                throw new Exception("Database error while attempting to create a Dungeon floor.");
            }
        }
    }
}
