//**********************************************************************************************************************
//  Messages
//  A collection of the generated messages

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
        //****************** Operation
        public int Count() {
            return _messages.Count();
        }

        public List<Message> Get() {
            return _messages;
        }

        //Add additional message to message log
        public void Add(Message message) {
            _messages.Add(message);
        }
    }
}
