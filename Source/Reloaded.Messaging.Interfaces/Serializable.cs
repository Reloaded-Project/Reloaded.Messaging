using System.Runtime.CompilerServices;
using Reloaded.Messaging.Interfaces.Message;

namespace Reloaded.Messaging.Interfaces
{
    /// <summary>
    /// An extension class providing serialization support to implementers of <see cref="ISerializable"/>.
    /// </summary>
    public static class Serializable
    {
        /// <summary>
        /// Serializes and compresses the current instance of the class or struct
        /// using the serializer and compressor defined by the <see cref="ISerializable"/>.
        /// </summary>
        public static byte[] Serialize<TSerializable>(this TSerializable serializable) where TSerializable : ISerializable
        {
            var serializer = serializable.GetSerializer();
            var compressor = serializable.GetCompressor();

            byte[] serialized = serializer.Serialize(ref serializable);
            if (compressor != null)
                return compressor.Compress(serialized);

            return serialized;
        }

        /// <summary>
        /// Decompresses and deserializes the current instance of the class or struct using the
        /// serializer and compressor defined by the <see cref="ISerializable"/>.
        /// </summary>
        public static ISerializable Deserialize<TType>(this TType serializable, byte[] bytes) where TType : ISerializable
        {
            var compressor = serializable.GetCompressor();
            var serializer = serializable.GetSerializer();

            byte[] decompressed = bytes;
            if (compressor != null)
                decompressed = compressor.Decompress(bytes);

            return serializer.Deserialize<TType>(decompressed);
        }

        /// <summary>
        /// Decompresses and deserializes the current instance of the class or struct using the
        /// serializer and compressor defined by the <see cref="ISerializable"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializable Deserialize<TType>(byte[] bytes) where TType : ISerializable, new()
        {
            var serializable = new TType();
            return Deserialize(serializable, bytes);
        }
    }
}
