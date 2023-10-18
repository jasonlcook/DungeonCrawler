using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;

namespace BlazorDungeonCrawler.Server.Models {
    public class Floors {
        private readonly List<Floor> _floors;

        public Floors() {
            _floors = new();
        }

        public Floors(List<SharedFloor> sharedFloors) {
            _floors = new();

            foreach (SharedFloor floor in sharedFloors) {
                _floors.Add(new Floor(floor));
            }
        }

        public int Count() {
            return _floors.Count();
        }

        public void Add(Floor floor) {
            _floors.Add(floor);
        }

        private List<Tile> GetTiles(int currentFloor) {
            if (_floors != null) {
                Floor? floor = _floors.Where(l => l.Depth == currentFloor).FirstOrDefault();
                if (floor != null && floor.Id != Guid.Empty) {
                    return floor.Tiles;
                } else {
                    throw new Exception("Dungeon Floor response was badly formed.");
                }
            } else {
                throw new Exception("Could not place tiles as Dungeon response was badly formed.");
            }
        }

        public List<SharedFloor> SharedModelMapper() {
            List<SharedFloor> sharedFloors = new();

            foreach (var floor in _floors) {
                sharedFloors.Add(floor.SharedModelMapper());
            }

            return sharedFloors;
        }
    }
}