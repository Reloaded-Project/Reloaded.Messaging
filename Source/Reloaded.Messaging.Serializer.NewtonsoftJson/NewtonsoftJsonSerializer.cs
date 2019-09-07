using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Serializer.NewtonsoftJson
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        /// <summary>
        /// Serialization options.
        /// </summary>
        public JsonSerializerSettings Options { get; private set; }

        /// <summary>
        /// Creates the System.Text.Json based serializer.
        /// </summary>
        /// <param name="serializerOptions">Options to use for serialization/deserialization.</param>
        public NewtonsoftJsonSerializer(JsonSerializerSettings serializerOptions)
        {
            Options = serializerOptions;
        }

        /// <inheritdoc />
        public TStruct Deserialize<TStruct>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<TStruct>(Encoding.UTF8.GetString(serialized), Options);
        }

        /// <inheritdoc />
        public byte[] Serialize<TStruct>(ref TStruct item)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item, Options));
        }
    }
}
