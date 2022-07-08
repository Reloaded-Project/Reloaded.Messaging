using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Serializer.ReloadedMemory;
using System;
using Reloaded.Messaging.Interfaces.Utilities;

namespace Reloaded.Messaging.Tests.Messages;

public struct Vector3 : IMessage<Vector3, UnmanagedReloadedMemorySerializer<Vector3>, NullCompressor>, IEquatable<Vector3>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3;
    public UnmanagedReloadedMemorySerializer<Vector3> GetSerializer() => new();
    public NullCompressor? GetCompressor() => null;

    public float X;
    public float Y;
    public float Z;

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3)obj);
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