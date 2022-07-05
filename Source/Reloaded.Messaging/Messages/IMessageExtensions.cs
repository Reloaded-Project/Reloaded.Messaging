using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Messages;

/// <summary>
/// Hosts various extension methods related to the <see cref="IMessage{TMessageType}"/> interface.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Retrieves the message type (key) for a given type.
    /// </summary>
    /// <param name="message">The message to get type for.</param>
    /// <returns>Used to instantiate and get message type (key) as single liner in source code.</returns>
    public static TMessageType GetMessageType<TMessageType>(this IMessage<TMessageType> message) where TMessageType : unmanaged
    {
        return message.GetMessageType();
    }
}