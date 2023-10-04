using System.Data.Entity;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Data {
    internal class MessagesContext : DbContext {
        public DbSet<Message> Messages { get; set; }

        public MessagesContext() : base("DungeonContext") { }
    }
}
