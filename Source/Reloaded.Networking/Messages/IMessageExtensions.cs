namespace Reloaded.Messaging.Messages
{
    public static class MessageExtensions
    {
        public static TMessageType GetMessageType<TMessageType>(this IMessage<TMessageType> message) where TMessageType : unmanaged
        {
            return message.GetMessageType();
        }
    }
}
