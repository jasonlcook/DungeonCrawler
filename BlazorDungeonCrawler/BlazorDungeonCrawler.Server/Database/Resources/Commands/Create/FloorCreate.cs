using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class FloorCreate  {
        protected readonly DungeonDbContext _dbContext;

        public FloorCreate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Create(Guid dungeonId, Floor floor) {
            try {
                Dungeon? attachedDungeon = _dbContext.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();

                if (attachedDungeon != null) {
                    attachedDungeon.Floors = new();
                    attachedDungeon.Floors.Add(floor);

                    _dbContext.Floors.Add(attachedDungeon.Floors.First());
                }

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to create a Dungeon floor.");
            }
        }
    }
}
