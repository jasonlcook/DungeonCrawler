using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class AdventurerCreate {
        public static void Create(Adventurer adventurer) {
            using (var context = new AdventurerContext()) {
                context.Adventurers.Add(adventurer);
                context.SaveChanges();
            }
        }
    }
}
