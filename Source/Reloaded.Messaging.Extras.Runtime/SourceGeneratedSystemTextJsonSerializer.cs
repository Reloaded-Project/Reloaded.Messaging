namespace Reloaded.Messaging.Extras.Runtime;

#if NET6_0_OR_GREATER
using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Reloaded.Messaging.Interfaces;
using System.Buffers;
using System.Runtime.CompilerServices;

/// <inheritdoc />
public struct SourceGeneratedSystemTextJsonSerializer<TStruct> : ISerializer<TStruct>
{
    private readonly JsonTypeInfo<TStruct> _typeInfo;

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    /// <param name="typeInfo">Source generated JSON type information.</param>
    public SourceGeneratedSystemTextJsonSerializer(JsonTypeInfo<TStruct> typeInfo)
    {
        _typeInfo = typeInfo;
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public TStruct Deserialize(Span<byte> serialized)
    {
        return JsonSerializer.Deserialize<TStruct>(serialized, _typeInfo)!;
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
        var write = Pool.JsonWriterPerThread();
        write.Reset(writer);
        JsonSerializer.Serialize(write, item, _typeInfo);
    }
}
#endif