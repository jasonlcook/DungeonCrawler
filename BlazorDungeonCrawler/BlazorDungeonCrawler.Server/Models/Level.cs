using SharedLevel = BlazorDungeonCrawler.Shared.Models.Level;


namespace BlazorDungeonCrawler.Server.Models {
    public class Level {
        public  Guid Id { get; private set; }
        public int Depth { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Tile> Tiles { get; private set; }

        public Level(int depth) {
            Id = Guid.NewGuid();

            Depth = depth;

            Tiles = new();

            GetRowsAndColumnsForCurrentDepth();
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

        public SharedLevel SharedModelMapper() {
            return new SharedLevel() {
                Id = this.Id,
                Depth = this.Depth,
                Rows = this.Rows, 
                Columns = this.Columns
            };
        }
    }
}