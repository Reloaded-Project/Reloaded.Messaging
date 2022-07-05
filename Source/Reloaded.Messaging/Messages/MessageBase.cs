namespace Reloaded.Messaging.Messages;

/// <summary>
/// The base class for the <see cref="Message{TMessageType,TStruct}"/> class, used for dealing with message type specific things.
/// </summary>
/// <typeparam name="TMessageType"></typeparam>
public unsafe class MessageBase<TMessageType> where TMessageType : unmanaged
{
    /// <summary>
    /// Retrieves the message type by  raw array of values.
    /// </summary>
    /// <param name="serializedBytes">The raw serialized bytes containing the message.</param>
    /// <returns>The type of the message.</returns>
    public static TMessageType GetMessageType(byte[] serializedBytes)
    {
        fixed (byte* arrayPtr = serializedBytes)
        {
            return *(TMessageType*) arrayPtr;
        }
    }
}