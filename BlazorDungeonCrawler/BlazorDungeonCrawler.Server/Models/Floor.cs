using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Server.Models {
    public class Floor {
        public Guid Id { get; private set; }
        public int Depth { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Tile> Tiles { get; set; }

        public Floor() { }

        public Floor(int depth) {
            Id = Guid.NewGuid();

            Depth = depth;

            Tiles = new();

            GetRowsAndColumnsForCurrentDepth();
        }

        public Floor(SharedFloor floor) {
            Id = floor.Id;
            Depth = floor.Depth;
            Rows = floor.Rows;
            Columns = floor.Columns;

            Tiles = new();
            foreach (SharedTile tile in floor.Tiles) {
                Tiles.Add(new Tile(tile));
            }
        }

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

        public SharedFloor SharedModelMapper() {
            SharedFloor floor = new() {
                Id = this.Id,
                Depth = this.Depth,
                Rows = this.Rows,
                Columns = this.Columns
            };

            floor.Tiles = new();

            foreach (Tile tile in this.Tiles) {
                floor.Tiles.Add(tile.SharedModelMapper());
            }

            return floor;
        }
    }
}