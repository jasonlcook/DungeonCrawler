using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Floor {

        [Key]
        public Guid Id { get; set; } 


        public int Depth { get; set; }


        public List<Tile> Tiles { get; set; }


        public int Rows { get; set; } 
        public int Columns { get; set; }

        public Floor() {
            Id = Guid.Empty;
            Tiles = new();
        }
    }
}
