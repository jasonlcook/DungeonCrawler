using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public static class MonstersCreate {
        public static void Create(DungeonDbContext context, Guid tileId, List<Monster> monsters) {
            try {
                Tile? attachedTile = context.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (attachedTile != null && attachedTile.Monsters != null) {
                    foreach (Monster monster in monsters) {
                        attachedTile.Monsters.Add(monster);
                    }

                    foreach (Monster monster in attachedTile.Monsters) {
                        context.Monsters.Add(monster);
                    }
                }

                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Monster create failed.");

            }
        }
    }
}
