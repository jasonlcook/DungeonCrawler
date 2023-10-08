using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

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

        public SharedTile SharedModelMapper() {
            SharedTile tile = new SharedTile() {
                Id = this.Id,
                Row = this.Row,
                Column = this.Column,
                Type = this.Type,
                Current = this.Current,
                Hidden = this.Hidden,
                Selectable = this.Selectable,
                FightWon = this.FightWon
            };

            if (this.Monsters.Count > 0) {
                foreach (Monster monster in this.Monsters) {
                    tile.Monsters.Add(monster.SharedModelMapper());
                }
            }

            return tile;
        }
    }
}