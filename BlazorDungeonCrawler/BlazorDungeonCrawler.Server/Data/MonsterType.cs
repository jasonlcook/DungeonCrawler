using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Server.Data {

    //todo: add how many monsters in a group
    public class MonsterType {

        public string Name { get; set; } 


        public int LevelStart { get; set; }
        public int LevelEnd { get; set; }

        public int MaxPackNumber { get; set; }


        public int HealthDiceCount { get; set; }
        public int DamageDiceCount { get; set; }
        public int ProtectionDiceCount { get; set; } 


        public string Documentation { get; set; }


        public MonsterType() {
            Name = string.Empty;
            Documentation = string.Empty;
        }
    }
}