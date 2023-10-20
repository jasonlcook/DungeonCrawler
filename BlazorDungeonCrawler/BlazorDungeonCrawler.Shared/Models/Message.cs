//Adventure message log 
using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Message {
        [Key]
        public Guid Id { get; set; }


        public double Datestamp { get; set; }


        public string Text { get; set; }


        public List<int> Dice { get; set; }

        public List<Message> Children { get; set; }

        public Message() {
            Id = Guid.Empty;
            Text = string.Empty;
            Dice = new();
            Children = new();
        }
    }
}
