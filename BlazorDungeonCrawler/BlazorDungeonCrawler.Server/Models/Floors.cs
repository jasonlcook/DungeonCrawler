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
        //****************** Operation
        
        //Add additional floor to the dungeon
        public void Add(Floor floor) {
            _floors.Add(floor);
        }
    }
}