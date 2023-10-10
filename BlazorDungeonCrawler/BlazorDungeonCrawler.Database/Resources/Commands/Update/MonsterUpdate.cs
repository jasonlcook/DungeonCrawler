using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Update {
    public static class MonsterUpdate {
        public static void Update(Monster monster) {
            try {
                using (var context = new DungeonContext()) {
                    context.Monsters.AddOrUpdate(monster);
                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Monster update failed.");

            }
        }
    }
}
