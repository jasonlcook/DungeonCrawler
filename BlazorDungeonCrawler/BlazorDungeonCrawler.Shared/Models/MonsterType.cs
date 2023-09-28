using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {

    //todo: add how many monsters in a group
    public class MonsterType {        
        [Key]
        //As this class not generated in code it is given a new Id on construction
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = "";

        public int LevelStart { get; set; } = 0;
        public int LevelEnd { get; set; } = 0;

        public int HealthDiceCount { get; set; } = 0;
        public int DamageDiceCount { get; set; } = 0;
        public int ProtectionDiceCount { get; set; } = 0;

        public string Documentation { get; set; } = "";
    }
}