﻿using BlazorDungeonCrawler.Shared.Models;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Message {
        public Guid Id { get; private set; }
        public double Datestamp { get; set; }
        public string Text { get; set; }
        public List<int>? Dice { get; private set; }

        public Message(string text) {
            Id = Guid.NewGuid();
            Datestamp = (DateTime.Now.ToUniversalTime() - new DateTime(2023, 10, 1)).TotalSeconds;
            Text = text;
            Dice = null;
        }

        public Message(string text, int die) : this(text) {
            Dice = new List<int> { die };
        }

        public Message(string text, List<int> dice) : this(text) {
            Dice = dice;
        }

        public SharedMessage SharedModelMapper() {
            SharedMessage sharedMessage = new SharedMessage() {
                Id = this.Id,
                Datestamp = this.Datestamp,
                Text = this.Text
            };

            if (Dice != null && Dice.Count > 0) {
                foreach (int die in this.Dice) {
                    sharedMessage.Dice.Add(die);
                }
            }

            return sharedMessage;
        }
    }
}