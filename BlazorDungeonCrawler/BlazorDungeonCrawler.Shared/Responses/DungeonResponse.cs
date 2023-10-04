using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Responses
{
    public class DungeonResponse
    {
        public bool Success { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public Dungeon Dungeon { get; set; } = new Dungeon();
    }

}
