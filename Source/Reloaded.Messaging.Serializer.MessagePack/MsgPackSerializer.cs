using MessagePack;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.MessagePack
{
    public class MsgPackSerializer : ISerializer
    {
        /// <summary>
        /// Uses LZ4 compression for serialization.
        /// </summary>
        public bool UseLZ4 { get; private set; }

        /// <summary>
        /// Any custom resolver to pass to MessagePack.
        /// Default is <see cref="MessagePack.Resolvers.ContractlessStandardResolver.Instance"/>
        /// </summary>
        public IFormatterResolver Resolver { get; private set; } = global::MessagePack.Resolvers.ContractlessStandardResolver.Instance;

        /// <summary>
        /// Creates a new instance of the MessagePack serializer.
        /// </summary>
        /// <param name="useLz4">Uses MessagePack's serializer with LZ4 compression.</param>
        /// <param name="resolver">
        ///     Custom resolver to pass to MessagePack, default is "Contractless Resolver"
        ///     (<see cref="MessagePack.Resolvers.ContractlessStandardResolver.Instance"/>).
        /// </param>
        public MsgPackSerializer(bool useLz4, IFormatterResolver resolver = null)
        {
            UseLZ4 = useLz4;
            if (resolver != null)
                Resolver = resolver;
        }


        /// <inheritdoc />
        public TStruct Deserialize<TStruct>(byte[] serialized)
        {
            return UseLZ4 ? LZ4MessagePackSerializer.Deserialize<TStruct>(serialized, Resolver) : 
                            MessagePackSerializer.Deserialize<TStruct>(serialized, Resolver);
        }


        /// <inheritdoc />
        public byte[] Serialize<TStruct>(ref TStruct item)
        {
            return UseLZ4 ? LZ4MessagePackSerializer.Serialize(item, Resolver) :
                            MessagePackSerializer.Serialize(item, Resolver);
        }
    }
}
