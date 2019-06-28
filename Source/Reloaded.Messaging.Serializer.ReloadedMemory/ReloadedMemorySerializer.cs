using Reloaded.Memory;
using Reloaded.Messaging.Serialization;

namespace Reloaded.Messaging.Serializer.ReloadedMemory
{
    public class ReloadedMemorySerializer : ISerializer
    {
        /// <summary>
        /// Marshals structures if set to true however is significantly slower.
        /// Note: Marshalling also allows you to serialize Classes with [StructLayout] attribute.
        /// </summary>
        public bool MarshalValues { get; set; }

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

        public TStruct Deserialize<TStruct>(byte[] serialized)
        {
            Struct.FromArray(serialized, out TStruct value, 0, MarshalValues);
            return value;
        }

        public byte[] Serialize<TStruct>(ref TStruct item)
        {
            byte[] bytes = Struct.GetBytes(ref item, MarshalValues);
            return bytes;
        }
    }
}
