using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class MessagesCreate  {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public MessagesCreate(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Create(Guid dungeonId, List<Message> messages) {
            try {
                Dungeon? attachedDungeon = _dbContext.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();
                if (attachedDungeon != null) {
                    attachedDungeon.Messages = new();

                    foreach (Message message in messages) {
                        attachedDungeon.Messages.Add(message);
                    }

                    foreach (Message message in attachedDungeon.Messages) {
                        _dbContext.Messages.Add(message);
                    }
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New Messages created");
            } catch (Exception ex) {
                _logger.LogError($"MessagesCreate Create Error: {ex.Message}");
                throw new Exception("Database error while attempting to create a Messages.");
            }
        }
    }
}
