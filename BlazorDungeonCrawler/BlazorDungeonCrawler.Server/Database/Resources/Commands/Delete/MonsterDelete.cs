using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Delete {
    public class MonsterDelete : IDisposable {
        protected readonly DungeonDbContext _dbContext;

        public MonsterDelete(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Delete(Guid mmonsterId) {
            try {
                Monster mmonster = _dbContext.Monsters.Where(m => m.Id == mmonsterId).First();

                _dbContext.Monsters.Remove(mmonster);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to delete a Monster.");
            }
        }

        public void Dispose() {
            //todo: dispose current context 
        }
    }
}
