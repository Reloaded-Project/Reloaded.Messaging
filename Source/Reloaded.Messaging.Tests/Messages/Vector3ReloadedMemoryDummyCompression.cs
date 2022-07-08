using System;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.ReloadedMemory;

namespace Reloaded.Messaging.Tests.Messages;

public struct Vector3ReloadedMemoryDummyCompression : IMessage<Vector3ReloadedMemoryDummyCompression, UnmanagedReloadedMemorySerializer<Vector3ReloadedMemoryDummyCompression>, NullCompressor>, IEquatable<Vector3ReloadedMemoryDummyCompression>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3ReloadedMemoryDummyCompression;
    public UnmanagedReloadedMemorySerializer<Vector3ReloadedMemoryDummyCompression> GetSerializer() => new();
    public NullCompressor? GetCompressor() => new();

    public float X;
    public float Y;
    public float Z;

    public Vector3ReloadedMemoryDummyCompression(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3ReloadedMemoryDummyCompression other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3ReloadedMemoryDummyCompression)obj);
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