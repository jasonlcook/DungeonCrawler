using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class LevelCreate {
        public static void Create(Level level) {
            using (var context = new LevelContext()) {
                context.Levels.Add(level);
                context.SaveChanges();
            }
        }
    }
}
