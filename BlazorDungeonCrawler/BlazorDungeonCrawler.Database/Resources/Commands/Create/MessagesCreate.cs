using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class MessagesCreate {
        public static void Create(List<Message> messages) {
            using (var context = new MessagesContext()) {
                context.Messages.AddRange(messages);
                context.SaveChanges();
            }
        }
    }
}
