//  version
using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;

namespace BlazorDungeonCrawler.Server.Models {
    public class Dungeon {
        //****************************
        //***************** Attributes
        public Guid Id { get; set; }

        public Adventurer Adventurer { get; set; }

        
        public int Depth { get; set; }
        public Floors Floors { get; set; }


        public Messages Messages { get; set; }


        public string ApiVersion { get; set; }


        public bool MacGuffinFound { get; set; }


        public bool StairsDiscovered { get; set; }


        public bool InCombat { get; set; }
        public Guid CombatTile { get; set; }

        public bool CombatInitiated { get; set; }


        public bool GameOver { get; set; }


        //****************************
        //*************** Constructors
        public Dungeon() {
            this.Id = Guid.NewGuid();
            this.Adventurer = new();
            this.Floors = new();
            this.Messages = new();
            this.ApiVersion = string.Empty;
        }

        //******************** Mapping

        //  DB > Class
        public Dungeon(SharedDungeon dungeon) {
            this.Id = dungeon.Id;
            this.Depth = dungeon.Depth;

            this.Adventurer = new Adventurer(dungeon.Adventurer);
            this.Floors = new Floors(dungeon.Floors);
            this.Messages = new Messages(dungeon.Messages);

            this.ApiVersion = dungeon.ApiVersion;
            this.MacGuffinFound = dungeon.MacGuffinFound;
            this.StairsDiscovered = dungeon.StairsDiscovered;
            this.InCombat = dungeon.InCombat;
            this.CombatTile = dungeon.CombatTile;
            this.CombatInitiated = dungeon.CombatInitiated;
            this.GameOver = dungeon.GameOver;
        }

        //  Class > DB
        public SharedDungeon SharedModelMapper() {
            return new() {
                Id = this.Id,
                Depth = this.Depth,

                Adventurer = this.Adventurer.SharedModelMapper(),
                Floors = this.Floors.SharedModelMapper(),
                Messages = this.Messages.SharedModelMapper(),

                ApiVersion = this.ApiVersion,
                MacGuffinFound = this.MacGuffinFound,
                StairsDiscovered = this.StairsDiscovered,
                InCombat = this.InCombat,
                CombatTile = this.CombatTile,
                CombatInitiated = this.CombatInitiated,
                GameOver = this.GameOver
            };
        }
    }
}
