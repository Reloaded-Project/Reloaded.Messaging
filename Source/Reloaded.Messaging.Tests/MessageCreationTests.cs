using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Tests.Messages;
using System;
using Reloaded.Messaging.Compressor.ZStandard;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces;
using Xunit;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.MessagePack;
using Reloaded.Messaging.Serializer.ReloadedMemory;

namespace Reloaded.Messaging.Tests;

public class MessageCreationTests
{
    #region Baseline Tests
    [Fact]
    public unsafe void CreateMessage_Baseline_WithReloadedMemoryNoCompression()
    {
        // Arrange
        var structure = new Vector3(0.5f, 1.0f, 2.0f);
        CreateMessage_Serializer_Common<Vector3, UnmanagedReloadedMemorySerializer<Vector3>, NullCompressor>(ref structure);
    }

    [Fact]
    public unsafe void CreateMessage_Baseline_WithReloadedMemoryDummyCompression()
    {
        // Arrange
        var structure = new Vector3ReloadedMemoryDummyCompression(0.5f, 1.0f, 2.0f);
        using var message = MessageWriter<Vector3ReloadedMemoryDummyCompression, UnmanagedReloadedMemorySerializer<Vector3ReloadedMemoryDummyCompression>, NullCompressor>.Serialize(ref structure);

        // Act & Assert
        var sizeEqual = (sizeof(Vector3) + HeaderReader.CompressedHeaderSize) == message.Span.Length;
        Assert.True(sizeEqual, "Size after packing does not match expected size.");

        HeaderReader.ReadHeader(message.Span, out var messageType, out var compressedSize, out var headerSize);
        Assert.True(structure.GetMessageType().Equals(messageType), "Invalid message type.");
        Assert.True(compressedSize == sizeof(Vector3ReloadedMemoryDummyCompression), "Compressed size should return invalid if no compression is used.");

        var deserialize = new MessageReader<Vector3ReloadedMemoryDummyCompression, UnmanagedReloadedMemorySerializer<Vector3ReloadedMemoryDummyCompression>, NullCompressor>(in structure);
        var deserialized = deserialize.Deserialize(message.Span.Slice(headerSize), compressedSize);
        Assert.True(structure.Equals(deserialized), "Vectors should be equal after deserialize.");
    }
    #endregion

    #region Serializer Tests
    [Fact]
    public unsafe void CreateMessage_WithMessagePack()
    {
        // Arrange
        var structure = new Vector3MessagePackNoCompression(0.5f, 1.0f, 2.0f);
        CreateMessage_Serializer_Common<Vector3MessagePackNoCompression, MessagePackSerializer<Vector3MessagePackNoCompression>, NullCompressor>(ref structure);
    }

    [Fact]
    public unsafe void CreateMessage_WithSystemTextJson()
    {
        // Arrange
        var structure = new Vector3SystemTextJson(0.5f, 1.0f, 2.0f);
        CreateMessage_Serializer_Common<Vector3SystemTextJson, SystemTextJsonSerializer<Vector3SystemTextJson>, NullCompressor>(ref structure);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public unsafe void CreateMessage_WithSystemTextJsonSourceGen()
    {
        // Arrange
        var structure = new Vector3SystemTextJsonSourceGenerated(0.5f, 1.0f, 2.0f);
        CreateMessage_Serializer_Common<Vector3SystemTextJsonSourceGenerated, SourceGeneratedSystemTextJsonSerializer<Vector3SystemTextJsonSourceGenerated>, NullCompressor>(ref structure);
    }
#endif

    #endregion

    #region Compressor Tests

    [Fact]
    public unsafe void CreateMessage_WithZStandard()
    {
        // Arrange
        var structure = new Vector3ZStandard(0.5f, 1.0f, 2.0f);
        CreateMessage_Compressor_Common<Vector3ZStandard, UnmanagedReloadedMemorySerializer<Vector3ZStandard>, ZStandardCompressor>(ref structure);
    }

    [Fact]
    public unsafe void CreateMessage_WithBrotli()
    {
        // Arrange
        var structure = new Vector3Brotli(0.5f, 1.0f, 2.0f);
        CreateMessage_Compressor_Common<Vector3Brotli, UnmanagedReloadedMemorySerializer<Vector3Brotli>, BrotliCompressor>(ref structure);
    }

    #endregion

    /// <summary>
    /// Common function for testing serializers.
    /// </summary>
    private unsafe void CreateMessage_Serializer_Common<TStruct, TSerializer, TCompressor>(ref TStruct value) where TStruct : IMessage<TStruct, TSerializer, TCompressor>, IEquatable<TStruct>, new()
        where TSerializer : ISerializer<TStruct>
        where TCompressor : ICompressor
    {
        // Arrange
        using var message = MessageWriter<TStruct, TSerializer, TCompressor>.Serialize(ref value);

        // Act & Assert
        HeaderReader.ReadHeader(message.Span, out var messageType, out var sizeAfterDecompression, out var headerSize);
        Assert.True(value.GetMessageType().Equals(messageType), "Invalid message type.");
        Assert.True(sizeAfterDecompression == -1, "Compressed size should return invalid if no compression is used.");

        var deserialize = new MessageReader<TStruct, TSerializer, TCompressor>(in value);
        var deserialized = deserialize.Deserialize(message.Span.Slice(headerSize), -1);
        Assert.True(value.Equals(deserialized), "Values should be equal after deserialize.");
    }

    /// <summary>
    /// Common function for testing serializers.
    /// </summary>
    private unsafe void CreateMessage_Compressor_Common<TStruct, TSerializer, TCompressor>(ref TStruct value) where TStruct : IMessage<TStruct, TSerializer, TCompressor>, new() 
        where TSerializer : ISerializer<TStruct> 
        where TCompressor : ICompressor
    {
        // Arrange
        using var message = MessageWriter<TStruct, TSerializer, TCompressor>.Serialize(ref value);

        // Act & Assert
        HeaderReader.ReadHeader(message.Span, out var messageType, out var sizeAfterDecompression, out var headerSize);
        Assert.True(value.GetMessageType().Equals(messageType), "Invalid message type.");

        var deserialize  = new MessageReader<TStruct, TSerializer, TCompressor>(in value);
        var deserialized = deserialize.Deserialize(message.Span.Slice(headerSize), sizeAfterDecompression);
        Assert.True(value.Equals(deserialized), "Values should be equal after deserialize.");
    }
}