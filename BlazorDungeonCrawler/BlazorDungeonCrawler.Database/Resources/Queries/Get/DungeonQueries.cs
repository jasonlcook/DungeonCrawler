using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Queries.Get {
    public static class DungeonQueries {
        public static Dungeon? Get(Guid dungeonId) {
            try {
                using (var context = new DungeonContext()) {
                    return context.Dungeons.Include("Adventurer").Include("Level").Include("Level.Tiles").Include("Level.Tiles.Monsters").Include("Messages").Where(d => d.Id == dungeonId).FirstOrDefault();                    
                }
            } catch (Exception ex) {
                throw new Exception("Dungeon retrieval failed.");
            }
        }
    }
}
