using Reloaded.Memory;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.ReloadedMemory
{
    public class ReloadedMemorySerializer : ISerializer
    {
        /// <summary>
        /// Marshals structures if set to true however is significantly slower.
        /// Note: Marshalling also allows you to serialize Classes with [StructLayout] attribute.
        /// </summary>
        public bool MarshalValues { get; private set; }

        /// <summary>
        /// Creates the Reloaded.Memory based serializer.
        /// </summary>
        /// <param name="marshalValues">
        ///     Marshals structures if set to true however is significantly slower.
        ///     Note: Marshalling also allows you to serialize Classes with [StructLayout] attribute.
        /// </param>
        public ReloadedMemorySerializer(bool marshalValues)
        {
            MarshalValues = marshalValues;
        }

        /// <inheritdoc />
        public TStruct Deserialize<TStruct>(byte[] serialized)
        {
            Struct.FromArray(serialized, out TStruct value, MarshalValues, 0);
            return value;
        }

        /// <inheritdoc />
        public byte[] Serialize<TStruct>(ref TStruct item)
        {
            byte[] bytes = Struct.GetBytes(ref item, MarshalValues);
            return bytes;
        }
    }
}
