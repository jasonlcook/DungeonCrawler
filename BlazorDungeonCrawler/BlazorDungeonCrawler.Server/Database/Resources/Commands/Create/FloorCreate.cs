using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create
{
    public static class FloorCreate
    {
        public static void Create(Guid dungeonId, Floor floor)
        {
            try
            {
                using (var context = new DungeonContext())
                {
                    Dungeon? attachedDungeon = context.Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();
                    if (attachedDungeon != null)
                    {
                        attachedDungeon.Floors = new();
                        attachedDungeon.Floors.Add(floor);

                        context.Floors.Add(attachedDungeon.Floors.First());
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Message create failed.");

            }
        }
    }
}
