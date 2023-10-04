using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Messages {
        private readonly List<Message> _messages;

        public Messages()
        {
            _messages = new();
        }

        public List<SharedMessage> SharedModelMapper() {
            List<SharedMessage> sharedMessages = new();

            foreach (var message in _messages) {
                SharedMessage sharedMessage =new SharedMessage() {
                    Id = message.Id,
                    Index = message.Index,
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
    }
}
