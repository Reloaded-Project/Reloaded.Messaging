using Reloaded.Messaging.Interfaces;
using System;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.MessagePack;

namespace Reloaded.Messaging.Tests.Messages;

public struct Vector3MessagePackNoCompression : IMessage<Vector3MessagePackNoCompression, MessagePackSerializer<Vector3MessagePackNoCompression>, NullCompressor>, IEquatable<Vector3MessagePackNoCompression>
{
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3MessagePackNoCompression;
    public MessagePackSerializer<Vector3MessagePackNoCompression> GetSerializer() => new();
    public NullCompressor? GetCompressor() => default;

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3MessagePackNoCompression(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Auto-implemented by R#
    public bool Equals(Vector3MessagePackNoCompression other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Vector3MessagePackNoCompression)obj);
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