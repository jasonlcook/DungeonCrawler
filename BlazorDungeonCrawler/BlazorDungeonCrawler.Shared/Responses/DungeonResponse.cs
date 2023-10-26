using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Responses {
    public class DungeonResponse {
        public bool Success { get; set; }
        public string ErrorMessages { get; set; }
        public Dungeon Dungeon { get; set; }

        public DungeonResponse() {
            ErrorMessages = string.Empty;
            Dungeon = new();
        }
    }
}
