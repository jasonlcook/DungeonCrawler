using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update
{
    public static class TilesUpdate
    {
        public static void Update(List<Tile> tiles)
        {
            try
            {
                using (var context = new DungeonContext())
                {
                    foreach (var tile in tiles)
                    {
                        context.Tiles.AddOrUpdate(tile);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tile update failed.");

            }
        }
    }
}
