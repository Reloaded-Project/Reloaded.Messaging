using MessagePack;
using Reloaded.Messaging.Messages;

namespace Reloaded.Messaging.Tests.Struct
{
    [MessagePackObject]
    public class StringMessage : IMessage<MessageType>
    {
        public MessageType GetMessageType() => MessageType.String;

        [Key(0)]
        public string Text { get; set; }

        public StringMessage(string text)
        {
            Text = text;
        }
    }
}
