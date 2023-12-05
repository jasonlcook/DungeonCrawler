//**********************************************************************************************************************
//  Floors
//  A collection of the dungeon floors currently discovered 

using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;

namespace BlazorDungeonCrawler.Server.Models {
    public class Floors {
        //****************************
        //***************** Attributes
        private readonly List<Floor> _floors;               //Collection of floors


        //****************************
        //*************** Constructors
        public Floors() {
            _floors = new();
        }

        //******************** Mapping

        //  DB > Class
        public Floors(List<SharedFloor> sharedFloors) {
            _floors = new();

            foreach (SharedFloor floor in sharedFloors) {
                _floors.Add(new Floor(floor));
            }
        }

        //  Class > DB
        public List<SharedFloor> SharedModelMapper() {
            List<SharedFloor> sharedFloors = new();

            foreach (var floor in _floors) {
                sharedFloors.Add(floor.SharedModelMapper());
            }

            return sharedFloors;
        }


        //****************************
        //****************** Accessors

        //********************* Floors        
        
        //  add a child element
        public void Add(Floor floor) {
            _floors.Add(floor);
        }

        //********************** Floor

        //  return floor at depth
        public Floor Get(int depth) {
            if (_floors == null || _floors.Count == 0) { throw new ArgumentNullException("Dungeon Floors"); }

            foreach (Floor floor in _floors) {
                if (floor.Depth == depth) {
                    return floor;
                }
            }

            return new();
        }

        //  update a child element
        public void Update(Floor _floor) {
            Floor floor;
            for (int i = 0; i < _floors.Count; i++)
            {
                floor = _floors[i];
                if (floor.Id == _floor.Id) {
                    floor = _floor;
                }
            }
        }
    }
}