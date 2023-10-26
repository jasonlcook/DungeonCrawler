using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public class MonstersCreate : IDisposable {
        protected readonly DungeonDbContext _dbContext;

        public MonstersCreate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Create(Guid tileId, List<Monster> monsters) {
            try {
                Tile? attachedTile = _dbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (attachedTile != null && attachedTile.Monsters != null) {
                    foreach (Monster monster in monsters) {
                        attachedTile.Monsters.Add(monster);
                    }

                    foreach (Monster monster in attachedTile.Monsters) {
                        _dbContext.Monsters.Add(monster);
                    }
                }

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to create a Monster.");
            }
        }

        public void Dispose() {
            //todo: dispose current context 
        }
    }
}
