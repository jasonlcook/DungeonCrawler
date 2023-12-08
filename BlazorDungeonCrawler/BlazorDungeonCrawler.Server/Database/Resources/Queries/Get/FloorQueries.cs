using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Queries.Get {
    public class FloorQueries {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public FloorQueries(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Floor?> Get(Guid dungeonId, int depth) {
            if (dungeonId == Guid.Empty) { throw new ArgumentNullException("Floor query get without dungeon Id"); }
            if (depth <= 0) { throw new ArgumentNullException("Floor query get without depth"); }

            try {
                _logger.LogInformation("Retrieving Floor");

                return await _dbContext.Floors
                    .Where(f => f.DungeonId == dungeonId)
                    .Where(f => f.Depth == depth)
                    .Include("Tiles")
                    .Include("Tiles.Monsters")
                    .FirstOrDefaultAsync();
            } catch (Exception ex) {
                _logger.LogError($"FloorQueries Get Error: {ex.Message}");
                throw new Exception("Database error while attempting to retrieve the Floor.");
            }
        }
    }
}
