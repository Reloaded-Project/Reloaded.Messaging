namespace Reloaded.Messaging.Tests.Messages;

#if NET6_0_OR_GREATER
using Reloaded.Messaging.Interfaces;
using System;
using System.Text.Json;
using Reloaded.Messaging.Interfaces.Utilities;
using System.Text.Json.Serialization;
using Reloaded.Messaging.Extras.Runtime;
[JsonSourceGenerationOptionsAttribute(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Vector3SystemTextJsonSourceGenerated))]
internal partial class Vector3JsonContext : JsonSerializerContext
{
}

public struct Vector3SystemTextJsonSourceGenerated : IMessage<Vector3SystemTextJsonSourceGenerated, SourceGeneratedSystemTextJsonSerializer<Vector3SystemTextJsonSourceGenerated>, NullCompressor>, IEquatable<Vector3SystemTextJsonSourceGenerated>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3SystemTextJson;
    public SourceGeneratedSystemTextJsonSerializer<Vector3SystemTextJsonSourceGenerated> GetSerializer() => new(Vector3JsonContext.Default.Vector3SystemTextJsonSourceGenerated);
    public NullCompressor? GetCompressor() => default;

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3SystemTextJsonSourceGenerated(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3SystemTextJsonSourceGenerated other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3SystemTextJsonSourceGenerated)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            hashCode = (hashCode * 397) ^ Z.GetHashCode();
            return hashCode;
        }
    }
}
#endif