using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        [Key]
        public Guid Id { get; set; }

        public Adventurer? Adventurer { get; set; }

        public int Depth { get; set; }

        public List<Floor>? Floors { get; set; }

        public List<Message>? Messages { get; set; }


        public string ApiVersion { get; set; }


        public bool MacGuffinFound { get; set; }


        public bool StairsDiscovered { get; set; }


        public bool InCombat { get; set; }
        public Guid CombatTile { get; set; }

        public bool CombatInitiated { get; set; }

        public bool GameOver { get; set; }

        [NotMapped]
        public bool NewDungeon { get; set; }

        public Dungeon() {
            Id = Guid.Empty;

            CombatTile = Guid.Empty;

            ApiVersion = string.Empty;
        }
    }
}