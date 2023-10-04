using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        [Key]
        public Guid Id { get; set; }

        public Adventurer Adventurer { get; set; }

        public Level Level { get; set; }

        public List<Message> Messages { get; set; }


        public string ApiVersion { get; set; }


        public bool MacGuffinFound { get; set; }


        public bool InCombat { get; set; }
        public Guid CombatTile { get; set; }

        public bool CombatInitiated { get; set; }

        public Dungeon() {
            Id = Guid.Empty;
            Adventurer = new();
            Level = new();
            Messages = new();
            CombatTile = Guid.Empty;
            ApiVersion = string.Empty;
        }
    }
}