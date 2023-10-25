using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create {
    public static class MessagesCreate {
        public static void Create(DungeonDbContext context, Guid dungeonId, List<Message> messages) {
            try {
                Dungeon? attachedDungeon = context.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();
                if (attachedDungeon != null) {
                    attachedDungeon.Messages = new();

                    foreach (Message message in messages) {
                        attachedDungeon.Messages.Add(message);
                    }

                    foreach (Message message in attachedDungeon.Messages) {
                        context.Messages.Add(message);
                    }
                }

                context.SaveChanges();
            } catch (Exception ex) {
                throw new Exception("Message create failed.");

            }
        }
    }
}
