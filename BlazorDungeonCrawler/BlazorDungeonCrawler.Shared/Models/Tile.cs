using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Tile {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;

        public List<Monster> Monsters { get; set; } = new List<Monster>();
    }
}
