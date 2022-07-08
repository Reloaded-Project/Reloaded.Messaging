using Reloaded.Messaging.Interfaces;
using System;
using Reloaded.Messaging.Utilities;

namespace Reloaded.Messaging.Messages;

/// <summary>
/// Struct used for deserializing of messages.
/// </summary>
/// <typeparam name="TStruct">The structure represented by the message.</typeparam>
/// <typeparam name="TSerializer">Type of serializer used.</typeparam>
/// <typeparam name="TCompressor">Type of compressor used.</typeparam>
public unsafe struct MessageReader<TStruct, TSerializer, TCompressor> where TStruct : IMessage<TStruct, TSerializer, TCompressor> 
    where TSerializer : ISerializer<TStruct> 
    where TCompressor : ICompressor
{
    private TCompressor? _compressor;
    private TSerializer _serializer;

    /// <summary>
    /// Deserializes messages with specified type.
    /// </summary>
    /// <param name="sample">A sample structure to extract compressor and serializer from.</param>
    public MessageReader(in TStruct sample)
    {
        _compressor = sample.GetCompressor();
        _serializer = sample.GetSerializer();
    }

    /// <summary>
    /// Deserializes a given set of bytes into a usable struct.
    /// </summary>
    /// <param name="serializedBytes">The raw bytes containing the message, without message header.</param>
    /// <param name="decompressedSize">Expected size after decompression, ignored if no compression will be used.</param>
    public TStruct Deserialize(Span<byte> serializedBytes, int decompressedSize)
    {
        // Get decompressor.
        if (_compressor != null)
        {
            // Decompress
            using var decompressedRental = new ArrayRental<byte>(decompressedSize);
            var decompressedSpan = decompressedRental.Span;
            _compressor.Decompress(serializedBytes, decompressedSpan);

            // Deserialize.
            return _serializer.Deserialize(decompressedSpan);
        }

        return _serializer.Deserialize(serializedBytes);
    }
}