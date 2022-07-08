using Reloaded.Messaging.Interfaces.Message;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Common interface shared by individual messages.
/// </summary>
public interface IMessage<TStruct, out TSerializer, out TCompressor> : ISerializable<TStruct, TSerializer, TCompressor> 
    where TSerializer : ISerializer<TStruct> 
    where TCompressor : ICompressor
{
    /// <summary>
    /// Returns the unique message type/id for this message.
    /// </summary>
    sbyte GetMessageType();
}