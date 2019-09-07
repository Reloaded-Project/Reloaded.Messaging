using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Messages
{
    public unsafe class Message<TMessageType, TStruct> : MessageBase<TMessageType> where TStruct : IMessage<TMessageType>, new() where TMessageType : unmanaged
    {
        // ReSharper disable StaticMemberInGenericType
        private static ISerializer DefaultSerializer { get; set; }
        private static ICompressor DefaultCompressor { get; set; }
        private static bool DefaultCompressorSet { get; set; }
        // ReSharper restore StaticMemberInGenericType

        public TStruct      ActualMessage { get; set; }

        public Message(TStruct message)
        {
            ActualMessage = message;
        }

        /// <summary>
        /// Serializes the current instance and returns an array of bytes representing the instance.
        /// </summary>
        public byte[] Serialize()
        {
            // Perform serialization.
            var serializer = GetSerializer();
            var message = ActualMessage;
            var encodedMessage = serializer.Serialize(ref message);

            // Allocate memory for result and write header.
            var result = new byte[encodedMessage.Length + sizeof(TMessageType)];
            var resultSpan = result.AsSpan();
            var messageType = ActualMessage.GetMessageType();

#if (USE_NATIVE_SPAN_API)
            var readOnlyMessageType = MemoryMarshal.CreateReadOnlySpan(ref messageType, sizeof(TMessageType));
            var readOnlyMessageTypeBytes = MemoryMarshal.AsBytes(readOnlyMessageType);
            readOnlyMessageTypeBytes.CopyTo(resultSpan);
#else
            byte* bytes = (byte*)Unsafe.AsPointer(ref messageType);
            var readOnlyMessageTypeBytes = new Span<byte>(bytes, sizeof(TMessageType)); 
            readOnlyMessageTypeBytes.CopyTo(resultSpan);
#endif

            // Append serialized data.
            resultSpan = resultSpan.Slice(sizeof(TMessageType));
            encodedMessage.AsSpan().CopyTo(resultSpan);

            var compressor = GetCompressor();
            if (compressor != null)
                result = compressor.Compress(result);

            return result;
        }

        /// <summary>
        /// Deserializes a given set of bytes into a usable struct.
        /// </summary>
        public static TStruct Deserialize(byte[] serializedBytes)
        {
            // Get decompressor.
            var compressor = GetCompressor();
            if (compressor != null)
                serializedBytes = compressor.Decompress(serializedBytes);

            // Get serializer
            var serializer = GetSerializer();

            // Read messagepack message.
            var messageSegment = serializedBytes.AsSpan(sizeof(TMessageType)).ToArray();
            var message = serializer.Deserialize<TStruct>(messageSegment);

            return message;

            // Note: No need to read MessageType. MessageType was only necessary to link a message to correct handler.
        }

        private static ISerializer GetSerializer()
        {
            if (Overrides.SerializerOverride.TryGetValue(typeof(TStruct), out ISerializer value))
                return value;

            if (DefaultSerializer == null)
            {
                var defaultStruct = new TStruct();
                DefaultSerializer = ((IMessage<TMessageType>)defaultStruct).GetSerializer();
            }

            return DefaultSerializer;
        }

        private static ICompressor GetCompressor()
        {
            if (Overrides.CompressorOverride.TryGetValue(typeof(TStruct), out ICompressor value))
                return value;

            if (! DefaultCompressorSet)
            {
                var defaultStruct = new TStruct();
                DefaultCompressor = ((IMessage<TMessageType>)defaultStruct).GetCompressor();
                DefaultCompressorSet = true;
            }

            return DefaultCompressor;
        }
    }
}
