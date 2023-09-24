namespace BlazorDungeonCrawler.Database.Migrations
{
    using BlazorDungeonCrawler.Shared.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Reflection.Metadata;

    internal sealed class Configuration : DbMigrationsConfiguration<BlazorDungeonCrawler.Database.DungeonCrawlerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlazorDungeonCrawler.Database.DungeonCrawlerContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
