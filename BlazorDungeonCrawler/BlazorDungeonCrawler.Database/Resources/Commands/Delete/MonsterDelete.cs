using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Delete {
    public static class MonsterDelete {
        public static void Delete(Guid mmonsterId) {
            try {
                using (var context = new DungeonContext()) {
                    Monster mmonster = context.Monsters.Where(m => m.Id == mmonsterId).First();

                    context.Monsters.Remove(mmonster);
                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Monsters delete failed.");

            }
        }
    }
}
