using Reloaded.Messaging.Interfaces;
using System;
using Reloaded.Messaging.Compressor.ZStandard;
using Reloaded.Messaging.Serializer.ReloadedMemory;

namespace Reloaded.Messaging.Tests.Messages;

public struct Vector3ZStandard : IMessage<Vector3ZStandard, UnmanagedReloadedMemorySerializer<Vector3ZStandard>, ZStandardCompressor>, IEquatable<Vector3ZStandard>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3ZStandard;
    public UnmanagedReloadedMemorySerializer<Vector3ZStandard> GetSerializer() => new();
    public ZStandardCompressor GetCompressor() => new();

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3ZStandard(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3ZStandard other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3ZStandard)obj);
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