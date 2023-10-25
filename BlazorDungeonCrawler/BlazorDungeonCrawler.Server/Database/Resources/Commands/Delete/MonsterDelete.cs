using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Delete {
    public static class MonsterDelete {
        public static void Delete(DungeonDbContext context, Guid mmonsterId) {
            try {
                Monster mmonster = context.Monsters.Where(m => m.Id == mmonsterId).First();

                context.Monsters.Remove(mmonster);
                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Monsters delete failed.");
            }
        }
    }
}
