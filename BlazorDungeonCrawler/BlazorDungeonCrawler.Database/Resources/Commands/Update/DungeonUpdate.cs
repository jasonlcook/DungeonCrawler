using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Update {
    public static class DungeonUpdate {
        public static void Update(Dungeon dungeon) {
            try {
                using (var context = new DungeonContext()) {
                    context.Dungeons.AddOrUpdate(dungeon);
                    context.SaveChanges();
                }
            } catch (Exception ex) {

                throw;
            }
        }
    }
}
