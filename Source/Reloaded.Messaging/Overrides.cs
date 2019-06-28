using System;
using System.Collections.Generic;
using Reloaded.Messaging.Compression;
using Reloaded.Messaging.Serialization;

namespace Reloaded.Messaging
{
    /// <summary>
    /// Class which provides, client-side the ability to override serializers and compressors for specified types.
    /// Use either for testing or benchmarking. 
    /// </summary>
    public static class Overrides
    {
        public static Dictionary<Type, ISerializer> SerializerOverride { get; } = new Dictionary<Type, ISerializer>();
        public static Dictionary<Type, ICompressor> CompressorOverride { get; } = new Dictionary<Type, ICompressor>();
    }
}
