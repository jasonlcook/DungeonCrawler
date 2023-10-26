//using BlazorDungeonCrawler.Shared.Models;

//namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
//    public class TilesCreate : IDisposable {
//        protected readonly DungeonDbContext _dbContext;

//        public TilesCreate(DungeonDbContext dbContext) {
//            _dbContext = dbContext;
//        }

//        public async Task Create( Guid floorId, List<Tile> tiles) {
//            try {
//                Floor? attachedFloor = _dbContext.Floors.Where(l => l.Id == floorId).FirstOrDefault();
//                if (attachedFloor != null && attachedFloor.Tiles != null) {
//                    attachedFloor.Tiles = new();
//                    foreach (Tile tile in tiles) {
//                        attachedFloor.Tiles.Add(tile);
//                    }

//                    foreach (Tile tile in attachedFloor.Tiles) {
//                        _dbContext.Tiles.Add(tile);
//                    }
//                }

//                await _dbContext.SaveChangesAsync();
//            } catch (Exception ex) {
//                throw new Exception("Tile create failed.");
//            }
//        }

//        public void Dispose() {
//            _dbContext.Dispose();
//        }
//    }
//}
