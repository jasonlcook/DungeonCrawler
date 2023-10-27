using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class MessagesCreate  {
        protected readonly DungeonDbContext _dbContext;

        public MessagesCreate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
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
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to create a Messages.");
            }
        }
    }
}
