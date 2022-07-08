using System;
using System.Buffers;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Defines the minimal interface necessary to bootstrap a 3rd party serializer.
/// </summary>
public interface ISerializer<TStruct>
{
    /// <summary>
    /// Deserializes the provided byte array into a concrete type.
    /// </summary>
    /// <param name="serialized">The data to deserialize.</param>
    TStruct Deserialize(Span<byte> serialized);

    /// <summary>
    /// Serializes the provided item into a byte array.
    /// </summary>
    /// <param name="item">The item to serialize to bytes.</param>
    /// <param name="writer">The writer into which the serialized message should be written to.</param>
    /// <returns>Serialized item.</returns>
    void Serialize(ref TStruct item, IBufferWriter<byte> writer);
}