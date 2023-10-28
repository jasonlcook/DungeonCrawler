using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class MonstersCreate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public MonstersCreate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Create(Guid tileId, List<Monster> monsters) {
            try {
                Tile? attachedTile = _dbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (attachedTile != null && attachedTile.Monsters != null) {
                    foreach (Monster monster in monsters) {
                        attachedTile.Monsters.Add(monster);
                    }

                    foreach (Monster monster in attachedTile.Monsters) {
                        _dbContext.Monsters.Add(monster);
                    }
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New Monsters created");
            } catch (Exception ex) {
                _logger.LogError($"MonstersCreate Create Error: {ex.Message}");
                throw new Exception("Database error while attempting to create a Monster.");
            }
        }
    }
}
