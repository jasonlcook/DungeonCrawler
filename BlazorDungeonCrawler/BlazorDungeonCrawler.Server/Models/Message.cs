using System.Collections.Generic;

namespace BlazorDungeonCrawler.Server.Models {
    public class Message {
        public Guid Id { get; private set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public  List<int>? Dice { get; private set; }

        public Message(int index, string text) {
            Id = Guid.NewGuid();
            Index = index;
            Text = text;
            Dice = null;
        }

        public Message(int index, string text, int die) : this(index, text) {
            Dice = new List<int> { die };
        }
    }
}