using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public static class DungeonUpdate {
        public static void Update(DungeonDbContext context, Dungeon dungeon) {
            try {
                context.Dungeons.Update(dungeon);
                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Dungeon update failed.");
            }
        }
    }
}
