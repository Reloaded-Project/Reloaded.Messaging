using System;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Reloaded.Messaging.Messages
{
    public unsafe class Message<TMessageType, TStruct> : MessageBase<TMessageType> where TStruct : IMessage<TMessageType> where TMessageType : unmanaged
    {
        public TStruct      ActualMessage { get; set; }

        public Message(TStruct message)
        {
            MessageType = ((IMessage<TMessageType>) message).GetMessageType();
            ActualMessage = message;
        }

        /// <summary>
        /// Serializes the current instance and returns an array of bytes representing the instance.
        /// </summary>
        public byte[] Serialize()
        {
            var encodedMessage = LZ4MessagePackSerializer.Serialize(ActualMessage);
            var result = new byte[encodedMessage.Length + sizeof(TMessageType)];

            var resultSpan = result.AsSpan();
            fixed (byte* resultSpanPtr = resultSpan)
            {
                Unsafe.Write(resultSpanPtr, MessageType);
            }

            resultSpan = resultSpan.Slice(sizeof(TMessageType));
            encodedMessage.AsSpan().CopyTo(resultSpan);

            return result;
        }

        /// <summary>
        /// Deserializes a given set of bytes into a usable struct.
        /// </summary>
        public static TStruct Deserialize(byte[] serializedBytes)
        {
            // Read messagepack message.
            var messageSegment = new ArraySegment<byte>(serializedBytes, sizeof(TMessageType), serializedBytes.Length - sizeof(TMessageType));
            var message = LZ4MessagePackSerializer.Deserialize<TStruct>(messageSegment);

            return message;

            // Note: No need to read MessageType. MessageType was only necessary to link a message to correct handler.
        }
    }

    /// <summary>
    /// Common interface shared by individual messages.
    /// </summary>
    public interface IMessage<TMessageType> where TMessageType : unmanaged
    {
        TMessageType GetMessageType();
    }
}
