using Reloaded.Messaging.Interfaces.Message;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Common interface shared by individual messages.
/// </summary>
public interface IMessage<TMessageType> : ISerializable where TMessageType : unmanaged
{
    /// <summary>
    /// Returns the unique message type/id for this message.
    /// </summary>
    TMessageType GetMessageType();
}