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

        //Add additional message to message log
        public void Add(Message message) {
            _messages.Add(message);
        }
    }
}
