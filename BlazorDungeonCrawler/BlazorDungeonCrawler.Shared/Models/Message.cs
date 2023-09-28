//Adventure message log 
using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Message {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;

        public int Index { get; set; } = 0;
        public string Text { get; set; } = "";
        public List<int> Dice { get; set; } = new List<int>();
    }
}
