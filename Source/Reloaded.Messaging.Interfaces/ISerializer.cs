namespace Reloaded.Messaging.Interfaces
{
    /// <summary>
    /// Defines the minimal interface necessary to bootstrap a 3rd party serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes the provided byte array into a concrete type.
        /// </summary>
        /// <typeparam name="TStruct">The type of the structure to deserialize.</typeparam>
        /// <param name="serialized">The data to deserialize.</param>
        TStruct Deserialize<TStruct>(byte[] serialized);

        /// <summary>
        /// Serializes the provided item into a byte array.
        /// </summary>
        /// <param name="item">The item to serialize to bytes.</param>
        /// <returns>Serialized item.</returns>
        byte[] Serialize<TStruct>(ref TStruct item);
    }
}
