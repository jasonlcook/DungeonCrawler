using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Responses
{
    public class TileResponse {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
        public Tile Tile { get; set; }

        public TileResponse() {
            ErrorMessages = new();
            Tile = new();
        }
    }

}
