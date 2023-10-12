using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Tile {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public DungeonEvents Type { get; set; }
        public bool Current { get; set; }
        public bool Hidden { get; set; }
        public bool Selectable { get; set; }
        public bool FightWon { get; set; }
        public List<Monster> Monsters { get; set; }

        public Tile() {
            Id = Guid.NewGuid();

            Type = DungeonEvents.Unknown;

            Current = false;
            Hidden = true;
            FightWon = false;

            Monsters = new();
        }

        public Tile(SharedTile tile) {
            Id = tile.Id;
            Row = tile.Row;
            Column = tile.Column;
            Type = tile.Type;
            Current = tile.Current;
            Hidden = tile.Hidden;
            Selectable = tile.Selectable;
            FightWon = tile.FightWon;

            Monsters = new();
            foreach (SharedMonster monster in tile.Monsters) {
                Monsters.Add(new Monster(monster));
            }
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