using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        [Key]
        public Guid Id { get; set; }

        public Adventurer? Adventurer { get; set; }

        public int Depth { get; set; }

        public List<Level>? Levels { get; set; }

        public List<Message>? Messages { get; set; }


        public string ApiVersion { get; set; }


        public bool MacGuffinFound { get; set; }


        public bool StairsDiscovered { get; set; }


        public bool InCombat { get; set; }
        public Guid CombatTile { get; set; }

        public bool CombatInitiated { get; set; }

        [NotMapped]
        public bool RefreshRequired { get; set; }

        public Dungeon() {
            Id = Guid.Empty;

            CombatTile = Guid.Empty;

            ApiVersion = string.Empty;

            RefreshRequired = false;
        }
    }
}