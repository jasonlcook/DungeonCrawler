using System.Collections.Generic;

namespace BlazorDungeonCrawler.Server.Models {
    public class Message {
        public Guid Id { get; private set; }
        public int Index { get; set; }
        public double Datestamp { get; set; }
        public string Text { get; set; }
        public  List<int>? Dice { get; private set; }

        public Message(int index, string text) {
            Id = Guid.NewGuid();
            Index = index;
            Datestamp = (DateTime.Now.ToUniversalTime() - new DateTime(2023, 10, 1)).TotalSeconds;
            Text = text;
            Dice = null;
        }

        public Message(int index, string text, int die) : this(index, text) {
            Dice = new List<int> { die };
        }
    }
}