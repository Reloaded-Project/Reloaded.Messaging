using System;
using System.Buffers;
using Reloaded.Memory;
using Reloaded.Messaging.Interfaces;

#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Reloaded.Messaging.Serializer.ReloadedMemory;

/// <summary>
/// Serializes messages using raw byte conversion with Reloaded.Memory.
/// </summary>
public unsafe struct UnmanagedReloadedMemorySerializer<TStruct> : ISerializer<TStruct> where TStruct : unmanaged
{
    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public TStruct Deserialize(Span<byte> serialized)
    {
        Struct.FromArray(serialized, out TStruct value);
        return value;
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
        var span = writer.GetSpan(sizeof(TStruct));
        Struct.GetBytes(ref item, span);
        writer.Advance(sizeof(TStruct));
    }
}