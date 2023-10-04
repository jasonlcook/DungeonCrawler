namespace BlazorDungeonCrawler.Database.Migrations
{
    using System.Data.Entity.Migrations;
 
    internal sealed class Configuration : DbMigrationsConfiguration<BlazorDungeonCrawler.Database.Data.DungeonContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
