using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class DungeonCreate : IDisposable {
        protected readonly DungeonDbContext _dbContext;

        public DungeonCreate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Create(Dungeon dungeon) {
            try {
                _dbContext.Dungeons.Add(dungeon);
                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to create a new Dungeon.");
            }
        }

        public void Dispose() {
            //todo: dispose current context 
        }
    }
}
