//**********************************************************************************************************************
//  Message
//  A message that will be dispayed in the client log

using BlazorDungeonCrawler.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Models {
    public class Message {
        //****************************
        //***************** Attributes
        public Guid Id { get; private set; }                //Database Id

        public double Datestamp { get; set; }               //A short datestamp used to order the messages

        public string Text { get; set; }                    //The message text

        public Messages Children { get; private set; } //Nested child messages (Used for action details)

        //  Dice
        public List<int>? SafeDice { get; private set; }    //Adventurer dice
        public List<int>? DangerDice { get; private set; }  //Monster dice

        public Guid? MessageId { get; set; }                //ForeignKey to possible parent record
        public Guid? DungeonId { get; set; }                //ForeignKey to possible parent record

        //****************************
        //*************** Constructors

        //Message with just text
        public Message(string text) {
            Id = Guid.NewGuid();
            Datestamp = (DateTime.Now.ToUniversalTime() - new DateTime(2023, 10, 1)).TotalSeconds;
            Text = text;
            Children = new();
        }

        //Message with text and single die
        public Message(string text, int? safeDie, int? dangerDie) : this(text) {
            if (safeDie != null) {
                SafeDice = new List<int>() { (int)safeDie };
            }

            if (dangerDie != null) {
                DangerDice = new() { (int)dangerDie };
            }
        }

        //Message with text and dice
        public Message(string text, List<int>? safeDice, List<int>? dangerDice) : this(text) {
            SafeDice = safeDice;
            DangerDice = dangerDice;
        }

        //******************** Mapping
        //  DB > Class
        public Message(SharedMessage message) {
            this.Id = message.Id;
            this.Datestamp = message.Datestamp;
            this.Text = message.Text;
            this.Children = new Messages(message.Children);

            if (message.SafeDice != null && !string.IsNullOrEmpty(message.SafeDice)) {
                this.SafeDice = message.SafeDice.Split(',')?.Select(Int32.Parse)?.ToList();
            }

            if (message.DangerDice != null && !string.IsNullOrEmpty(message.DangerDice)) {
                this.DangerDice = message.DangerDice.Split(',')?.Select(Int32.Parse)?.ToList();
            }

            MessageId = message.MessageId;
            DungeonId = message.DungeonId;
        }

        //  Class > DB
        public SharedMessage SharedModelMapper() {
            SharedMessage sharedMessage = new SharedMessage() {
                Id = this.Id,
                Datestamp = this.Datestamp,
                Text = this.Text,
                MessageId = this.MessageId,
                DungeonId = this.DungeonId
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

            if (Children != null && Children.Count() > 0) {
                List<SharedMessage> children = new();
                foreach (var message in Children.Get()) {
                    children.Add(message.SharedModelMapper());
                }

                sharedMessage.Children = children;
            }

            return sharedMessage;
        }

        //****************************
        //****************** Accessors

        //  add additional nested message for message action
        public void AddChild(Message message) {
            Children.Add(message);
        }
    }
}