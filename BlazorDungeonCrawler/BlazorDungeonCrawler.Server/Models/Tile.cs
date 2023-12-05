//**********************************************************************************************************************
//  Tile
//  Details of an individual tile that makes up the dungeon floor

using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Tile {
        //****************************
        //***************** Attributes

        public Guid Id { get; set; }                        //Database Id


        public DungeonEvents Type { get; set; }             //Current tyle type


        public bool Hidden { get; set; }                    //Is the tile type known 

        public bool Selectable { get; set; }                //Is the tile currently selectable (accessable by the adventurer)

        public bool Current { get; set; }                   //Is the Adventurer currently located on this tile

        public bool FightWon { get; set; }                  //Is there a fight currently ongoing on this tile


        public List<Monster> Monsters { get; set; }         //List of monsters currently on this tile


        //  Position 
        //  The location of the tile on the floor grid
        public int Row { get; set; }
        public int Column { get; set; }

        //****************************
        //*************** Constructors
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

        //******************** Mapping

        //  Class > DB
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

        //****************************
        //****************** Operation

        //  return monsters for this tile
        public List<Monster> GetMonsters() {
            if (Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floor tile"); }

            return Monsters.OrderBy(m => m.Index).ToList();
        }
    }
}