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
                sharedMessages.Add(message.SharedModelMapper());
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
