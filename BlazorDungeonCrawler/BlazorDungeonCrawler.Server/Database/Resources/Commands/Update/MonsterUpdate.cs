using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public static class MonsterUpdate {
        public static void Update(DungeonDbContext context, Monster monster) {
            try {
                context.Monsters.Update(monster);
                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Monster update failed.");

            }
        }
    }
}
