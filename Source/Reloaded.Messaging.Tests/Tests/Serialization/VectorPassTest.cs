using System;
using System.Threading;
using LiteNetLib;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serializer.MessagePack;
using Reloaded.Messaging.Serializer.ReloadedMemory;
using Reloaded.Messaging.Structs;
using Reloaded.Messaging.Tests.Init;
using Xunit;
using Vector3 = Reloaded.Messaging.Tests.Struct.MessagePack.Vector3;

namespace Reloaded.Messaging.Tests.Tests.Serialization
{
    public class VectorPassTest : IDisposable
    {
        private TestingHosts _hosts;

        public VectorPassTest()
        {
            _hosts = new TestingHosts();
        }

        public void Dispose()
        {
            Overrides.SerializerOverride.Remove(typeof(Vector3));
            Overrides.CompressorOverride.Remove(typeof(Vector3));
            _hosts.Dispose();
        }

        [Fact(Timeout = 1000)]
        public void MsgPackPassVector3()
        {
            Overrides.SerializerOverride[typeof(Vector3)] = new MsgPackSerializer(false);
            Overrides.CompressorOverride.Remove(typeof(Vector3));
            PassVector3();
        }

        [Fact(Timeout = 1000)]
        public void ReloadedPassVector3()
        {
            Overrides.SerializerOverride[typeof(Vector3)] = new ReloadedMemorySerializer(false);
            Overrides.CompressorOverride.Remove(typeof(Vector3));
            PassVector3();
        }

        private void PassVector3()
        {
            Vector3 message = new Vector3(1.0F, 235.0F, 100.0F);
            Vector3 delivered = default;

            // Message handling method
            void Handler(ref NetMessage<Vector3> netMessage)
            {
                delivered = netMessage.Message;
            }

            // Setup client
            _hosts.SimpleClient.MessageHandler.AddOrOverrideHandler<Vector3>(Handler);

            // Send Message.
            var vectorMessage = new Message<MessageType, Vector3>(message);
            var data = vectorMessage.Serialize();
            _hosts.SimpleServer.NetManager.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);

            while (delivered.Equals(default(Vector3)))
                Thread.Sleep(16);

            Assert.Equal(message, delivered);
        }
    }
}
