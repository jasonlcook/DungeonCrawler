using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Tile {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();


        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;

        
        public DungeonEvemts Type { get; set; } = DungeonEvemts.Unknown;

        
        public bool Current { get; set; } = true;
        public bool Hidden { get; set; } = true;
        public bool Selectable { get; set; } = false;


        public List<Monster>? Monsters { get; set; } = null;
    }
}
