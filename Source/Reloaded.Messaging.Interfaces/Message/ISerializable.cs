namespace Reloaded.Messaging.Interfaces.Message;

/// <summary>
/// An interface that provides serialization/deserialization and compression/decompression support for. 
/// </summary>
public interface ISerializable<TStruct, out TSerializer, out TCompressor> where TSerializer : ISerializer<TStruct>
                                                                          where TCompressor : ICompressor
{
    /// <summary>
    /// Returns the serializer for this specific type.
    /// </summary>
    TSerializer GetSerializer();

    /// <summary>
    /// Returns the compressor for this specific type.
    /// </summary>
    TCompressor? GetCompressor();
}