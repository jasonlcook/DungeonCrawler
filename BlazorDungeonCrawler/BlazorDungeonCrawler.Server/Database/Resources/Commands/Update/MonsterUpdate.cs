using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class MonsterUpdate  {
        protected readonly DungeonDbContext _dbContext;

        public MonsterUpdate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Update(Monster monster) {
            try {
                _dbContext.Monsters.Update(monster);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to update a Monster.");
            }
        }
    }
}
