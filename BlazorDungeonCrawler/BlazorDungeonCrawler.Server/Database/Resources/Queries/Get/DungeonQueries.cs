using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Queries.Get {
    public static class DungeonQueries {
        public static Dungeon? Get(DungeonDbContext context, Guid dungeonId) {
            try {
                return context.Dungeons.Include("Adventurer").Include("Floors").Include("Floors.Tiles").Include("Floors.Tiles.Monsters").Include("Messages").Include("Messages.Children").Include("Messages.Children.Children").Where(d => d.Id == dungeonId).FirstOrDefault();
            } catch (Exception ex) {
                throw new Exception("Dungeon retrieval failed.");
            }
        }
    }
}
