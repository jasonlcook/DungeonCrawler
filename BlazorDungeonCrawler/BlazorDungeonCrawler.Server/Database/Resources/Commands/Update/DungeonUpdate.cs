using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public  class DungeonUpdate  {
        protected readonly DungeonDbContext _dbContext;

        public DungeonUpdate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Update(Dungeon dungeon) {
            try {
                _dbContext.Dungeons.Update(dungeon);

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to update the Dungeon.");
            }
        }
    }
}
