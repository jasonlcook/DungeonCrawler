using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public static class DungeonCreate {
        public static void Create(DungeonDbContext context, Dungeon dungeon) {
            try {
                context.Dungeons.Add(dungeon);
                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Dungeon create failed.");
            }

        }
    }
}
