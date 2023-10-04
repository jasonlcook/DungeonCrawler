using System.ComponentModel.DataAnnotations;
using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Tile {

        [Key]
        public Guid Id { get; set; }


        public int Row { get; set; }
        public int Column { get; set; }


        public DungeonEvemts Type { get; set; }


        public bool Current { get; set; }
        public bool Hidden { get; set; }
        public bool Selectable { get; set; }


        public bool FightWon { get; set; }
        public List<Monster> Monsters { get; set; }

        public Tile() {
            Id = Guid.Empty;
            Monsters = new();

        }
    }
}
