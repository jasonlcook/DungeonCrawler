using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class MonsterCreate {
        public static void Create(Tile tile, List<Monster> monsters) {
            try {
                using (var context = new DungeonContext()) {
                    Tile? attachedTile = context.Tiles.Where(t => t.Id == tile.Id).FirstOrDefault();
                    if (attachedTile != null && attachedTile.Monsters != null)
                    {
                        foreach (Monster monster in monsters) {
                            attachedTile.Monsters.Add(monster);
                        }

                        foreach (Monster monster in attachedTile.Monsters) {
                            context.Monsters.Add(monster);
                        }
                    }

                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Monster create failed.");

            }
        }
    }
}
