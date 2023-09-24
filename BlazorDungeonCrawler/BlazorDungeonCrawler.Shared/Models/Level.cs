namespace BlazorDungeonCrawler.Shared.Models {
    public class Level {
        public int Depth { get; set; } = 0;

        public int Rows { get; set; } = 0;
        public int Columns { get; set; } = 0;

        public List<Tile> Tiles { get; set; } = new List<Tile>();
    }
}
