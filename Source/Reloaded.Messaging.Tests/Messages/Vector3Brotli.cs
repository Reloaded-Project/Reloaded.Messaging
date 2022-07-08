using Reloaded.Messaging.Interfaces;
using System;
using Reloaded.Messaging.Compressor.ZStandard;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Serializer.ReloadedMemory;

namespace Reloaded.Messaging.Tests.Messages;

public struct Vector3Brotli : IMessage<Vector3Brotli, UnmanagedReloadedMemorySerializer<Vector3Brotli>, BrotliCompressor>, IEquatable<Vector3Brotli>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3Brotli;
    public UnmanagedReloadedMemorySerializer<Vector3Brotli> GetSerializer() => new();
    public BrotliCompressor GetCompressor() => new();

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3Brotli(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3Brotli other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3Brotli)obj);
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