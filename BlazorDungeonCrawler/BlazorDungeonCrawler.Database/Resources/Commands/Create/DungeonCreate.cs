using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class DungeonCreate {
        public static void Create(Dungeon dungeon) {
            try {
                using (var context = new DungeonContext()) {
                    context.Dungeons.Add(dungeon);
                    context.SaveChanges();
                }
            } catch (Exception ex) {

                throw;
            }

        }
    }
}
