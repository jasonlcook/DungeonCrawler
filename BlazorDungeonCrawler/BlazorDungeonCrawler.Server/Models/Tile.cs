using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Tile {
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
            Id = Guid.NewGuid();

            Type = DungeonEvemts.Unknown;

            Current = false;
            Hidden = true;
            FightWon = false;

            Monsters = new();
        }
    }
}