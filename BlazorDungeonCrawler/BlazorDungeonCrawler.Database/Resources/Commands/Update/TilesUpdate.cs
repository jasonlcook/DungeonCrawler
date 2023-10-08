using System.Data.Entity;
using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Update {
    public static class TilesUpdate {
        public static void Update(List<Tile> tiles) {
            try {
                using (var context = new DungeonContext()) {
                    foreach (var tile in tiles) {
                        if(tile.Monsters.Count > 0) {
                            foreach (Monster monster in tile.Monsters) {
                                context.Monsters.AddOrUpdate(monster);
                                context.Entry(tile).State = EntityState.Modified;
                            }
                        }

                        context.Tiles.AddOrUpdate(tile);
                    }

                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Tile update failed.");

            }
        }
    }
}
