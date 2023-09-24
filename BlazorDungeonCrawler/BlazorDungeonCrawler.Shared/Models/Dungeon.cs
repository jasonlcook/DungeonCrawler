namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        //Id for the Dungeon session 
        public Guid Id { get; set; } = new Guid();

        public Adventurer Adventurer { get; set; } = new Adventurer();

        public Level Level { get; set; } = new Level();

        public bool MacGuffinFound { get; set; } = false;
    }
}