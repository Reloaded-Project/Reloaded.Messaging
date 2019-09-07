using System;
using Reloaded.Messaging.Compressor.ZStandard;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serializer.ReloadedMemory;
using Reloaded.Messaging.Tests.Struct;
using Xunit;
using ZstdNet;

namespace Reloaded.Messaging.Tests.Tests.Compression
{
    public class CompressionTest : IDisposable
    {
        public void Dispose()
        {
            Overrides.SerializerOverride.Remove(typeof(Vector3));
            Overrides.CompressorOverride.Remove(typeof(Vector3));
        }

        [Fact]
        public void CheckCompression()
        {
            var originalVector = new Vector3(235F, 10F, 5F);
            var vectorMessage = new Message<MessageType, Vector3>(originalVector);

            // Set serialization: Reloaded.
            // Set compression: None
            GC.Collect();
            Overrides.SerializerOverride[typeof(Vector3)] = new ReloadedMemorySerializer(false);
            Overrides.CompressorOverride.Remove(typeof(Vector3));

            var serializedUncompressed = vectorMessage.Serialize();
            var deserializedUncompressed = Message<MessageType, Vector3>.Deserialize(serializedUncompressed);
            Assert.Equal(originalVector, deserializedUncompressed);

            // Set compression: ZStandard
            Overrides.CompressorOverride[typeof(Vector3)] = new ZStandardCompressor(new CompressionOptions(22));
            var serializedZStandard = vectorMessage.Serialize();
            var deserializedZStandard = Message<MessageType, Vector3>.Deserialize(serializedZStandard);

            Assert.Equal(originalVector, deserializedZStandard);
            Assert.NotEqual(serializedUncompressed, serializedZStandard);

            // Note: Compression here acts negatively.
        }
    }
}
