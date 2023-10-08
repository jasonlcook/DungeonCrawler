using System.Data.Entity;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class MessageCreate {
        public static void Create(Dungeon dungeon, List<Message> messages) {
            try {
                using (var context = new DungeonContext()) {
                    context.Entry(dungeon).State = EntityState.Modified;

                    context.Messages.AddRange(messages);
                    context.SaveChanges();
                }
            } catch (Exception ex) {

                throw;
            }
        }
    }
}
