//**********************************************************************************************************************
//  Messages
//  A collection of the generated messages

using BlazorDungeonCrawler.Shared.Models;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Messages {
        //****************************
        //***************** Attributes
        private readonly List<Message> _messages;           //Collection of messages

        //****************************
        //*************** Constructors
        public Messages() {
            _messages = new();
        }

        //******************** Mapping

        //  DB > Class
        public Messages(List<SharedMessage> messages) {
            _messages = new();

            foreach (SharedMessage message in messages) {
                _messages.Add(new Message(message));
            }
        }

        //  Class > DB
        public List<SharedMessage> SharedModelMapper() {
            List<SharedMessage> sharedMessages = new();

            foreach (var message in _messages) {
                sharedMessages.Add(message.SharedModelMapper());
            }

            return sharedMessages;
        }

        //****************************
        //****************** Accessors

        //  return count of child elements
        public int Count() {
            return _messages.Count();
        }

        //  add a child element
        public void Add(Message message) {
            _messages.Add(message);
        }

        //  add child elements
        public void AddRange(Messages messages) {
            _messages.AddRange(messages.Get());
        }

        //  return all messages
        public List<Message> Get() {
            return _messages;
        }
    }
}
