using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update {
    public static class AdventurerUpdate {
        public static void Update(DungeonDbContext context, Adventurer adventurer) {
            try {
                context.Adventurers.Update(adventurer);
                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Adventurer update failed.");

            }
        }
    }
}
