namespace BlazorDungeonCrawler.Shared.Models {
    public class Monster {
        public Guid Id { get; set; } = new Guid();

        public string TypeName { get; set; } = "";

        public int Health { get; set; } = 0;

        public int Damage { get; set; } = 0;

        public int Protection { get; set; } = 0;
    }
}