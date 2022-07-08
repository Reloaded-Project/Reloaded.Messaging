using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Extras.Runtime;

/// <inheritdoc />
public struct SystemTextJsonSerializer<TStruct> : ISerializer<TStruct>
{
    /// <summary>
    /// Serialization options.
    /// </summary>
    public JsonSerializerOptions Options { get; private set; }

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    public SystemTextJsonSerializer()
    {
        Options = new JsonSerializerOptions();
    }

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    /// <param name="serializerOptions">Options to use for serialization/deserialization.</param>
    public SystemTextJsonSerializer(JsonSerializerOptions serializerOptions)
    {
        Options = serializerOptions;
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public TStruct Deserialize(Span<byte> serialized)
    {
        return JsonSerializer.Deserialize<TStruct>(serialized, Options)!;
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
        var write = Pool.JsonWriterPerThread();
        write.Reset(writer);
        JsonSerializer.Serialize(write, item, Options);
    }
}