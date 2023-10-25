using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Commands.Create
{
    public static class TilesCreate
    {
        public static void Create(Guid floorId, List<Tile> tiles)
        {
            try
            {
                using (var context = new DungeonContext())
                {
                    Floor? attachedFloor = context.Floors.Where(l => l.Id == floorId).FirstOrDefault();
                    if (attachedFloor != null && attachedFloor.Tiles != null)
                    {
                        attachedFloor.Tiles = new();
                        foreach (Tile tile in tiles)
                        {
                            attachedFloor.Tiles.Add(tile);
                        }

                        foreach (Tile tile in attachedFloor.Tiles)
                        {
                            context.Tiles.Add(tile);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tile create failed.");

            }
        }
    }
}
