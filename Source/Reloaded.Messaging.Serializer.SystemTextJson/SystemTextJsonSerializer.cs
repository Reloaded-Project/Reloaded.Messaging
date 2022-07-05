using System;
using System.Text.Json;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.SystemTextJson;

/// <inheritdoc />
public class SystemTextJsonSerializer : ISerializer
{
    /// <summary>
    /// Serialization options.
    /// </summary>
    public JsonSerializerOptions Options { get; private set; }

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    /// <param name="serializerOptions">Options to use for serialization/deserialization.</param>
    public SystemTextJsonSerializer(JsonSerializerOptions serializerOptions)
    {
        Options = serializerOptions;
    }

    /// <inheritdoc />
    public TStruct Deserialize<TStruct>(byte[] serialized)
    {
        return JsonSerializer.Deserialize<TStruct>(serialized, Options);
    }

    /// <inheritdoc />
    public byte[] Serialize<TStruct>(ref TStruct item)
    {
        return JsonSerializer.SerializeToUtf8Bytes(item, Options);
    }
}