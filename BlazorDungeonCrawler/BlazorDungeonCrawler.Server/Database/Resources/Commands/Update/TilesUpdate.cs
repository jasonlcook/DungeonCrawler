using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public class TilesUpdate : IDisposable {
        protected readonly DungeonDbContext _dbContext;

        public TilesUpdate(DungeonDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task Update(List<Tile> tiles) {
            try {
                foreach (var tile in tiles) {
                    _dbContext.Tiles.Update(tile);
                }

                await _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                //todo: log exception with Application Insights
                throw new Exception("Database error while attempting to update a Tile.");
            }
        }

        public void Dispose() {
            //todo: dispose current context 
        }
    }
}
