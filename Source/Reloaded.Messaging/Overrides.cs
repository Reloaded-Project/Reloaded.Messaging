using System;
using System.Collections.Generic;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging
{
    /// <summary>
    /// Class which provides, client-side the ability to override serializers and compressors for specified types.
    /// Use either for testing or benchmarking. 
    /// </summary>
    public static class Overrides
    {
        /// <summary>
        /// Allows you to override the serializer for a specific type.
        /// </summary>
        public static Dictionary<Type, ISerializer> SerializerOverride { get; } = new Dictionary<Type, ISerializer>();

        /// <summary>
        /// Allows you to override the compressor for a specific type.
        /// </summary>
        public static Dictionary<Type, ICompressor> CompressorOverride { get; } = new Dictionary<Type, ICompressor>();
    }
}
