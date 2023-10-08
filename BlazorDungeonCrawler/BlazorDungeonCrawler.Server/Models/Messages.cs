using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Messages {
        private readonly List<Message> _messages;

        public Messages() {
            _messages = new();
        }

        public int Count() {
            return _messages.Count();
        }

        public List<SharedMessage> SharedModelMapper() {
            List<SharedMessage> sharedMessages = new();

            foreach (var message in _messages) {
                SharedMessage sharedMessage = new() {
                    Id = message.Id,
                    Index = message.Index,
                    Datestamp = message.Datestamp,
                    Text = message.Text,
                };

                if (message.Dice != null) {
                    sharedMessage.Dice = message.Dice;
                }

                sharedMessages.Add(sharedMessage);
            }

            return sharedMessages;
        }

        public void Add(Message message) {
            _messages.Add(message);
        }

        public List<Message> GetAll() {
            return _messages.ToList();
        }
    }
}
