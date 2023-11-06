namespace BlazorDungeonCrawler.Server.Data {
    public class MonsterType {

        public string Name { get; set; } 


        public int FloorStart { get; set; }
        public int FloorEnd { get; set; }

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