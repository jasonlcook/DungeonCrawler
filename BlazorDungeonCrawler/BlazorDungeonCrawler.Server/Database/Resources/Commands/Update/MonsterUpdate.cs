using System.Data.Entity.Migrations;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Update
{
    public static class MonsterUpdate
    {
        public static void Update(Monster monster)
        {
            try
            {
                using (var context = new DungeonContext())
                {
                    context.Monsters.AddOrUpdate(monster);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Monster update failed.");

            }
        }
    }
}
