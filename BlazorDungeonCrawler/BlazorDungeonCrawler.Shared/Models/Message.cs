//Adventure message log 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Message {
        [Key]
        public Guid Id { get; set; }

        public double Datestamp { get; set; }

        public string Text { get; set; }

        public List<Message> Children { get; set; }

        public string SafeDice { get; set; }
        public string DangerDice { get; set; }

        [ForeignKey("Message")]
        public Guid? MessageId { get; set; }


        [ForeignKey("Dungeon")]
        public Guid? DungeonId { get; set; }


        public Message() {
            Id = Guid.Empty;
            Text = string.Empty;
            Children = new();
            SafeDice = string.Empty;
            DangerDice = string.Empty;
        }
    }
}
