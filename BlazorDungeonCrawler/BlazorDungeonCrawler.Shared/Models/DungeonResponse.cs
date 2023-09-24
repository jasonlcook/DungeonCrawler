namespace BlazorDungeonCrawler.Shared.Models {
    public class DungeonResponse {
        public bool Success { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public Dungeon Dungeon { get; set; } = new Dungeon();
    }

}
