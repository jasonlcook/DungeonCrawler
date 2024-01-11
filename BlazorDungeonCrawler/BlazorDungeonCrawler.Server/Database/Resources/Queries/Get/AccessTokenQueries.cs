using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Queries.Get {
    public class AccessTokenQueries {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public AccessTokenQueries(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AccessToken?> Get(string token) {
            if (token == string.Empty) { throw new ArgumentNullException("Access token query get without token"); }

            try {
                _logger.LogInformation("Retrieving token");

                return await _dbContext.AccessTokens.FirstOrDefaultAsync(t => t.Name == token);
            } catch (Exception ex) {
                _logger.LogError($"AccessTokenQueries Get Error: {ex.Message}");
                throw new Exception("Database error while attempting to retrieve the Access token.");
            }
        }
    }
}
