//**********************************************************************************************************************
//  Monster
//  Server version of the Database and Client class containing mothods for updating state and mapping between Shared
//  version

using BlazorDungeonCrawler.Shared.Models;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monster {
        //****************************
        //***************** Attributes
        public Guid Id { get; set; }                        //Database Id

        public int Index { get; set; }                      //Monster's place in party

        public string TypeName { get; set; }                //Monster display name
        

        public int Experience { get; set; }                 //Amount of experience gained by killing the monster


        public int Dexterity { get; set; }                  //Dexterity
        public int Health { get; set; }                     //Current health points
        public int Damage { get; set; }                     //Damage dealt to adventurer
        public int Protection { get; set; }                 //Protection from adventurer's attacks


        public Guid TileId { get; set; }                    //ForeignKey to parent record


        //  Counter position
        //  Each monster is displayed as a counter showing its current health.The location of the counter is set when the tile is generated.
        public int ClientX { get; set; }
        public int ClientY { get; set; }


        //****************************
        //*************** Constructors
        public Monster() {
            Id = Guid.NewGuid();
            TypeName = string.Empty;
        }

        public Monster(SharedMonster monster) {
            Id = monster.Id;
            Index = monster.Index;
            TypeName = monster.TypeName;
            Dexterity = monster.Dexterity;
            Health = monster.Health;
            Damage = monster.Damage;
            Protection = monster.Protection;
            Experience = monster.Experience;
            ClientX = monster.ClientX;
            ClientY = monster.ClientY;
            TileId = monster.TileId;
        }

        //******************** Mapping

        //  DB > Class
        public Monster ServerModelMapper(SharedMonster monster) {
            return new Monster(monster);
        }

        //  Class > DB
        public SharedMonster SharedModelMapper() {
            return new SharedMonster() {
                Id = this.Id,
                Index = this.Index,
                TypeName = this.TypeName,
                Dexterity = this.Dexterity,
                Health = this.Health,
                Damage = this.Damage,
                Protection = this.Protection,
                Experience = this.Experience,
                ClientX = this.ClientX,
                ClientY = this.ClientY,
                TileId = this.TileId
        };
        }
    }
}