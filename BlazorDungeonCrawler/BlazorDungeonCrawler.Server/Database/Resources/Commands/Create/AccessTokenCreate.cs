using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class AccessTokenCreate {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public AccessTokenCreate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Create(AccessToken accessToken) {
            try {
                _dbContext.AccessTokens.Add(accessToken);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New Access token created");
            } catch (Exception ex) {
                _logger.LogError($"AccessTokenCreate Create Error: {ex.Message}");
                throw new Exception("Database error while attempting to create a new Token.");
            }
        }
    }
}
