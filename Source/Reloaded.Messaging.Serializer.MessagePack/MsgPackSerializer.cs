using MessagePack;
using MessagePack.Resolvers;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.MessagePack;

/// <summary>
/// Serializer that uses MessagePack.
/// </summary>
public class MsgPackSerializer : ISerializer
{
    /// <summary>
    /// Any custom resolver to pass to MessagePack.
    /// Default is <see cref="ContractlessStandardResolver.Instance"/>
    /// </summary>
    public IFormatterResolver Resolver { get; private set; } = ContractlessStandardResolver.Instance;

    /// <summary>
    /// Options for the MessagePack serializer.
    /// </summary>
    public MessagePackSerializerOptions SerializerOptions { get; private set; } = MessagePackSerializerOptions.Standard;

    /// <summary>
    /// Creates a new instance of the MessagePack serializer.
    /// </summary>
    /// <param name="resolver">
    ///     Custom resolver to pass to MessagePack, default is "Contractless Resolver"
    ///     (<see cref="ContractlessStandardResolver.Instance"/>).
    /// </param>
    public MsgPackSerializer(IFormatterResolver resolver = null)
    {
        if (resolver != null)
            Resolver = resolver;
    }

    /// <inheritdoc />
    public TStruct Deserialize<TStruct>(byte[] serialized)
    {
        return MessagePackSerializer.Deserialize<TStruct>(serialized, SerializerOptions);
    }


    /// <inheritdoc />
    public byte[] Serialize<TStruct>(ref TStruct item)
    {
        return MessagePackSerializer.Serialize(item, SerializerOptions);
    }
}