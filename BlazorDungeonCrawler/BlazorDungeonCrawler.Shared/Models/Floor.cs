using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Floor {

        [Key]
        public Guid Id { get; set; } 


        public int Depth { get; set; }


        public List<Tile> Tiles { get; set; }


        public bool IsCurrent { get; set; }


        public int Rows { get; set; } 
        public int Columns { get; set; }


        [ForeignKey("Dungeon")]
        public Guid DungeonId { get; set; }


        public Floor() {
            Id = Guid.Empty;
            Tiles = new();
        }
    }
}
