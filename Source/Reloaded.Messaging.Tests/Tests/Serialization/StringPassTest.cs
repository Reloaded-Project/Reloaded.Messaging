using System;
using System.Threading;
using LiteNetLib;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serializer.MessagePack;
using Reloaded.Messaging.Serializer.ReloadedMemory;
using Reloaded.Messaging.Structs;
using Reloaded.Messaging.Tests.Init;
using Reloaded.Messaging.Tests.Struct.MessagePack;
using Xunit;

namespace Reloaded.Messaging.Tests.Tests.Serialization
{
    public class StringPassTest : IDisposable
    {
        private const string Message = "Test Message";
        private TestingHosts _hosts;

        public StringPassTest()
        {
            _hosts = new TestingHosts();
        }

        public void Dispose()
        {
            Overrides.SerializerOverride.Remove(typeof(StringMessage));
            Overrides.CompressorOverride.Remove(typeof(StringMessage));
            _hosts.Dispose();
        }

        [Fact(Timeout = 1000)]
        public void MsgPackPassString()
        {
            Overrides.SerializerOverride[typeof(StringMessage)] = new MsgPackSerializer(false);
            Overrides.CompressorOverride[typeof(StringMessage)] = null;
            PassString();
        }

        [Fact(Timeout = 1000)]
        public void ReloadedPassString()
        {
            Overrides.SerializerOverride[typeof(StringMessage)] = new ReloadedMemorySerializer(true);
            Overrides.CompressorOverride[typeof(StringMessage)] = null;
            PassString();
        }

        private void PassString()
        {
            string delivered = default;
            
            // Message handling method
            void Handler(ref NetMessage<StringMessage> netMessage)
            {
                delivered = netMessage.Message.Text;
            }

            // Setup client.
            _hosts.SimpleClient.MessageHandler.AddOrOverrideHandler<StringMessage>(Handler);

            // Send Message.
            var stringMessage = new Message<MessageType, StringMessage>(new StringMessage(Message));
            var data = stringMessage.Serialize();
            _hosts.SimpleServer.NetManager.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);

            while (delivered == default)
                Thread.Sleep(16);

            Assert.Equal(Message, delivered);
        }
    }
}
