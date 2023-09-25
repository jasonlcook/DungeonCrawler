//Adventure message log 
namespace BlazorDungeonCrawler.Shared.Models {
    public class Message {
        public Guid Id { get; set; } = new Guid();
        public int Index { get; set; } = 0;
        public string Text { get; set; } = "";
        public List<int> Dice { get; set; } = new List<int>();
    }
}
