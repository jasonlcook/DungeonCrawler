namespace BlazorDungeonCrawler.Server.Migrations {
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<BlazorDungeonCrawler.Server.Database.DungeonContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlazorDungeonCrawler.Server.Database.DungeonContext context) { }
    }
}
