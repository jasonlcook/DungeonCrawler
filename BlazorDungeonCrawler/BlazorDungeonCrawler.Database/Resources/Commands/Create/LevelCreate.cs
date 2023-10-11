using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class LevelCreate {
        public static void Create(Guid dungeonId, Level level) {
            try {
                using (var context = new DungeonContext()) {
                    Dungeon? attachedDungeon = context.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();
                    if (attachedDungeon != null) {
                        attachedDungeon.Levels = new();
                        attachedDungeon.Levels.Add(level);

                        context.Levels.Add(attachedDungeon.Levels.First());
                    }

                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Message create failed.");

            }
        }
    }
}
