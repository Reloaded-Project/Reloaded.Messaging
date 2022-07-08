using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Resolvers;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.MessagePack;

/// <summary>
/// Serializer that uses MessagePack.
/// </summary>
public struct MessagePackSerializer<TStruct> : ISerializer<TStruct>
{
    /// <summary>
    /// Options for the MessagePack serializer.
    /// </summary>
    public MessagePackSerializerOptions SerializerOptions { get; private set; } = MessagePackSerializerOptions.Standard;

    /// <summary>
    /// Creates a new instance of the MessagePack serializer.
    /// </summary>
    public MessagePackSerializer()
    {
        SerializerOptions = SerializerOptions.WithResolver(ContractlessStandardResolver.Instance);
    }

    /// <summary>
    /// Creates a new instance of the MessagePack serializer.
    /// </summary>
    /// <param name="resolver">
    ///     Custom resolver to pass to MessagePack, default instance uses "Contractless Resolver".
    /// </param>
    public MessagePackSerializer(IFormatterResolver? resolver = null)
    {
        SerializerOptions = SerializerOptions.WithResolver(resolver ?? ContractlessStandardResolver.Instance);
    }

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public unsafe TStruct Deserialize(Span<byte> serialized)
    {
        fixed (byte* dataPtr = &serialized[0])
        {
            // We have to tank potential heap allocation here.  
            // Hoping JIT escape analysis is smart enough not to heap allocate this one.  
            var manager = new UnmanagedMemoryManager<byte>(dataPtr, serialized.Length);
            return MessagePackSerializer.Deserialize<TStruct>(manager.Memory, SerializerOptions);
        }
    }

    /// <inheritdoc />
    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
        MessagePackSerializer.Serialize(writer, item, SerializerOptions);
    }
}