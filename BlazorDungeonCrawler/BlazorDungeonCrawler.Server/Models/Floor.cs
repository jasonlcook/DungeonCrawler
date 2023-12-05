//**********************************************************************************************************************
//  Floor
//  Each Dungeon floor is comprised of a grid hexagon of tiles, with each tile having an allocated potion within that grid.

using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Server.Models {
    public class Floor {
        //****************************
        //***************** Attributes
        public Guid Id { get; private set; }                //Database Id

        public int Depth { get; set; }                      //Current floor depth


        public Tiles Tiles { get; set; }               //the collection of tiles that make up the dungeon floor
        

        //  Details of the floor layout used to both populate the floor with the correct amount of tile and and position the floor on the client
        public int Rows { get; set; }
        public int Columns { get; set; }
        

        //****************************
        //*************** Constructors
        public Floor() {
            Id = Guid.Empty;
            Tiles = new();
        }

        public Floor(int depth) {
            Id = Guid.NewGuid();

            Depth = depth;

            Tiles = new();

            GetRowsAndColumnsForCurrentDepth();
        }

        //******************** Mapping

        //  DB > Class
        public Floor(SharedFloor floor) {
            Id = floor.Id;
            Depth = floor.Depth;
            Rows = floor.Rows;
            Columns = floor.Columns;

            Tiles = new();
            foreach (SharedTile tile in floor.Tiles) {
                Tiles.Add(new(tile));
            }
        }

        //  Class > DB
        public SharedFloor SharedModelMapper() {
            SharedFloor floor = new() {
                Id = this.Id,
                Depth = this.Depth,
                Rows = this.Rows,
                Columns = this.Columns
            };

            floor.Tiles = new();

            foreach (Tile tile in this.Tiles.Get()) {
                floor.Tiles.Add(tile.SharedModelMapper());
            }

            return floor;
        }

        //****************************
        //****************** Accessors

        //  retrieve tiles
        public Tiles GetTiles() {
            return Tiles;
        }

        //****************************
        //****************** Operation

        //  retrieve the floor size
        public void GetRowsAndColumnsForCurrentDepth() {
            switch (Depth) {
                case 1:
                    Rows = 3;
                    Columns = 3;
                    break;
                case 2:
                case 3:
                    Rows = 5;
                    Columns = 5;
                    break;
                case 4:
                case 5:
                    Rows = 7;
                    Columns = 7;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    Rows = 7;
                    Columns = 9;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("DungeonDepth");
            }
        }
    }
}