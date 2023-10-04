using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class TilesCreate {
        public static void Create(List<Tile> tiles) {
            using (var context = new TileContext()) {
                context.Tiles.AddRange(tiles);
                context.SaveChanges();
            }
        }
    }
}
