namespace BlazorDungeonCrawler.Shared.Models {

    //todo: add how many monsters in a group
    public class MonsterType {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";

        public int LevelStart { get; set; } = 0;

        public int LevelEnd { get; set; } = 0;

        public int HealthDiceCount { get; set; } = 0;
        public int DamageDiceCount { get; set; } = 0;
        public int ProtectionDiceCount { get; set; } = 0;
        public string Documentation { get; set; } = "";
    }
}