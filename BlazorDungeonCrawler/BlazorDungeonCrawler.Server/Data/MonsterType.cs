using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {

    //todo: add how many monsters in a group
    public class MonsterType {
        [Key]
        public Guid Id { get; set; }


        public string Name { get; set; } 


        public int LevelStart { get; set; }
        public int LevelEnd { get; set; }


        public int HealthDiceCount { get; set; }
        public int DamageDiceCount { get; set; }
        public int ProtectionDiceCount { get; set; } 


        public string Documentation { get; set; }


        public MonsterType() {
            Id = Guid.Empty;
            Name = string.Empty;
            Documentation = string.Empty;

        }
    }
}