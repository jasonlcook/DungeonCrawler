using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public static class TilesUpdate {
        public static void Update(DungeonDbContext context, List<Tile> tiles) {
            try {
                foreach (var tile in tiles) {
                    context.Tiles.Update(tile);
                }

                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Tile update failed.");
            }
        }
    }
}
