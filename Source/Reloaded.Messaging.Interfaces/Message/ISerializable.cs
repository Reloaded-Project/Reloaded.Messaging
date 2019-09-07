namespace Reloaded.Messaging.Interfaces.Message
{
    /// <summary>
    /// An interface that provides serialization/deserialization and compression/decompression support for. 
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Returns the serializer for this specific type.
        /// </summary>
        ISerializer GetSerializer();

        /// <summary>
        /// Returns the compressor for this specific type.
        /// </summary>
        ICompressor GetCompressor();
    }
}