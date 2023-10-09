using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Update {
    public static class AdventurerUpdate {
        public static void Update(Adventurer adventurer) {
            try {
                using (var context = new DungeonContext()) {
                    context.Adventurers.AddOrUpdate(adventurer);
                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Adventurer update failed.");

            }
        }
    }
}
