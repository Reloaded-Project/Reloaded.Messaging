using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.IO;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Message;
using Reloaded.Messaging.Messages.Disposables;
using Reloaded.Messaging.Utilities;

namespace Reloaded.Messaging.Messages;

/// <summary>
/// Writes single messages to be sent via the network.
/// </summary>
/// <typeparam name="TStruct">The structure represented by the message.</typeparam>
/// <typeparam name="TSerializer">Structure used to perform serialization.</typeparam>
/// <typeparam name="TCompressor">Structure used to perform compression.</typeparam>
public static unsafe class MessageWriter<TStruct, TSerializer, TCompressor> where TStruct : IMessage<TStruct, TSerializer, TCompressor>
    where TSerializer : ISerializer<TStruct> 
    where TCompressor : ICompressor
{
    /// <summary>
    /// Serializes the current instance and returns a disposable memory array of data representing the instance.
    /// </summary>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReusableSingletonMemoryStream Serialize(ref TStruct data)
    {
        // Get the compressor.
        // If there is no compressor, we can write data type directly and then ser
        var compressor = data.GetCompressor();
        if (compressor != null)
            return SerializeToRecyclableMemoryStream(ref data, Pool.MessageStreamPerThread(), compressor);
        else
            return SerializeToRecyclableMemoryStream(ref data, Pool.MessageStreamPerThread());
    }

    /// <summary>
    /// Serializes the current structure to, a provided <see cref="RecyclableMemoryStream"/> with compression.<br/><br/>
    /// Internal-ish API intended for benchmarking.
    /// </summary>
    /// <param name="data">The data to serialize.</param>
    /// <param name="messageStream">The memory stream to serialize to.</param>
    /// <param name="compressor">The compressor to use.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReusableSingletonMemoryStream SerializeToRecyclableMemoryStream(ref TStruct data, RecyclableMemoryStream messageStream, TCompressor compressor)
    {
        // Serialize message first.
        var serializer = data.GetSerializer();
        using var serializeStream = new ReusableSingletonMemoryStream(Pool.CompressionStreamPerThread());
        serializer.Serialize(ref data, serializeStream.Stream);
        var serializedSpan = serializeStream.Span;
        var decompressedSize = serializedSpan.Length;

        // Write data type.
        var messageTypePacked = data.GetMessageType() | HeaderReader.CompressionFlag;
        messageStream.WriteByte((byte)messageTypePacked);

        // Reserve space for compressed data.
        var compressedLengthPos = messageStream.Position;
        messageStream.Seek(HeaderReader.HeaderDecompressedDataSize, SeekOrigin.Current);
        var compressedDataStartPos = messageStream.Position;

        // Compress directly into stream.
        var maxCompressedSize = compressor.GetMaxCompressedSize((int)serializeStream.Stream.Length);
        var compressedSpan = messageStream.GetSpan(maxCompressedSize);
        int numCompressed = compressor.Compress(serializedSpan, compressedSpan);

        // Write compressed size
        messageStream.Seek(compressedLengthPos, SeekOrigin.Begin);
        if (!BitConverter.IsLittleEndian) // evaluated at JIT time, it's an intrinsic.
            decompressedSize = BinaryPrimitives.ReverseEndianness(decompressedSize);

        messageStream.Write(SpanExtensions.AsByteSpanFast(&decompressedSize));

        // Set length (to account for compressed # bytes).
        messageStream.SetLength(compressedDataStartPos + numCompressed);
        return new ReusableSingletonMemoryStream(messageStream);
    }

    /// <summary>
    /// Serializes the current structure to, a provided <see cref="RecyclableMemoryStream"/> without using compression.<br/><br/>
    /// Internal-ish API intended for benchmarking.
    /// </summary>
    /// <param name="data">The data to serialize.</param>
    /// <param name="messageStream">The memory stream to serialize to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReusableSingletonMemoryStream SerializeToRecyclableMemoryStream(ref TStruct data, RecyclableMemoryStream messageStream)
    {
        // If there is no compression, we can write type then data directly.
        messageStream.WriteByte((byte)data.GetMessageType());
        data.GetSerializer().Serialize(ref data, messageStream);
        return new ReusableSingletonMemoryStream(messageStream);
    }
}

/// <summary>
/// Extension methods for easier serialization and deserialization.
/// </summary>
public static unsafe class MessageWriterExtensions
{
    // Note: It's a mess but has no impact on JIT-ted code.

    /// <summary>
    /// Serializes the current instance and returns a disposable memory array of data representing the instance.
    /// </summary>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReusableSingletonMemoryStream Serialize<TStruct, TSerializer, TCompressor>(this IMessage<TStruct, TSerializer, TCompressor> dummy0, ref TStruct data, TSerializer? dummy1 = default, TCompressor? dummy2 = default) 
        where TStruct : IMessage<TStruct, TSerializer, TCompressor>
        where TSerializer : ISerializer<TStruct>
        where TCompressor : ICompressor
    {
       return MessageWriter<TStruct, TSerializer, TCompressor>.Serialize(ref data);
    }
}