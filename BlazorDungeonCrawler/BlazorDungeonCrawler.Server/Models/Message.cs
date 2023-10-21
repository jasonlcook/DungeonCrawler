using System.Text;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Message {
        public Guid Id { get; private set; }
        public double Datestamp { get; set; }
        public string Text { get; set; }
        public List<Message> Children { get; private set; }
        public List<int>? SafeDice { get; private set; }
        public List<int>? DangerDice { get; private set; }

        public Message(string text) {
            Id = Guid.NewGuid();
            Datestamp = (DateTime.Now.ToUniversalTime() - new DateTime(2023, 10, 1)).TotalSeconds;
            Text = text;
            Children = new();
        }

        public Message(string text, int? safeDie, int? dangerDie) : this(text) {
            if (safeDie != null) {
                SafeDice = new List<int>() { (int)safeDie };
            }

            if (dangerDie != null) {
                DangerDice = new() { (int)dangerDie };
            }
        }

        public Message(string text, List<int>? safeDice, List<int>? dangerDice) : this(text) {
            SafeDice = safeDice;
            DangerDice = dangerDice;
        }

        public void AddChild(Message message) {
            Children.Add(message);
        }

        public SharedMessage SharedModelMapper() {
            SharedMessage sharedMessage = new SharedMessage() {
                Id = this.Id,
                Datestamp = this.Datestamp,
                Text = this.Text
            };

            if (SafeDice != null && SafeDice.Count > 0) {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (int safeDie in this.SafeDice) {
                    stringBuilder.Append(safeDie + ",");
                }

                sharedMessage.SafeDice = stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }

            if (DangerDice != null && DangerDice.Count > 0) {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (int dangerDice in this.DangerDice) {
                    stringBuilder.Append(dangerDice + ",");
                }

                sharedMessage.DangerDice = stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }

            if (Children != null && Children.Count > 0) {
                List<SharedMessage> children = new();
                foreach (var message in Children) {
                    children.Add(message.SharedModelMapper());
                }

                sharedMessage.Children = children;
            }

            return sharedMessage;
        }
    }
}