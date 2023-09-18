namespace BlazorDungeonCrawler.Shared.Models {
    public class Monster {
        public int Id { get; set; } = 0;

        public string Name { get; set; } = "";

        public int LevelStart { get; set; } = 0;
        public int LevelEnd { get; set; } = 0;


        public int DiceHealth { get; set; } = 0;
        public int DiceDamage { get; set; } = 0;
        public int DiceProtection { get; set; } = 0;

        public string Documentation { get; set; } = "";
    }
}