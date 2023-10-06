using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Responses
{
    public class TilesResponse {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<Tile> Tiles { get; set; }

        public TilesResponse() {
            ErrorMessages = new();
            Tiles = new();
        }
    }

}
