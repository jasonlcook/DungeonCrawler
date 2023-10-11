using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class TilesCreate {
        public static void Create(Guid levelId, List<Tile> tiles) {
            try {
                using (var context = new DungeonContext()) {
                    Level? attachedLevel = context.Levels.Where(l => l.Id == levelId).FirstOrDefault();
                    if (attachedLevel != null && attachedLevel.Tiles != null)
                    {
                        attachedLevel.Tiles = new();
                        foreach (Tile tile in tiles) {
                            attachedLevel.Tiles.Add(tile);
                        }

                        foreach (Tile tile in attachedLevel.Tiles) {
                            context.Tiles.Add(tile);
                        }
                    }

                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Tile create failed.");

            }
        }
    }
}
