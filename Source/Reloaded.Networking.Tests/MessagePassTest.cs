using System.Net;
using System.Threading;
using LiteNetLib;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Structs;
using Reloaded.Messaging.Tests.Struct;
using Xunit;
using Vector3 = Reloaded.Messaging.Tests.Struct.Vector3;

namespace Reloaded.Messaging.Tests
{
    public class MessagePassTest
    {
        private const string DefaultPassword = "CutenessIsJustice";
        private SimpleHost<MessageType> _simpleServer;
        private SimpleHost<MessageType> _simpleClient;

        public MessagePassTest()
        {
            _simpleServer = new SimpleHost<MessageType>(true, DefaultPassword);
            _simpleClient = new SimpleHost<MessageType>(false, DefaultPassword);

            _simpleServer.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            _simpleClient.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            _simpleClient.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, _simpleServer.NetManager.LocalPort), DefaultPassword);

#if DEBUG
            _simpleServer.NetManager.DisconnectTimeout = int.MaxValue;
            _simpleClient.NetManager.DisconnectTimeout = int.MaxValue;
#endif
        }

        [Fact(Timeout = 1000)]
        public void PassString()
        {
            string message = "Test Message";
            string delivered = "";
            bool isDelivered = false;

            // Message handling method
            void Handler(ref NetMessage<StringMessage> netMessage)
            {
                delivered = netMessage.Message.Text;
                isDelivered = true;
            }

            // Setup client.
            _simpleClient.MessageHandler.AddOrOverrideHandler<StringMessage>(MessageType.String, Handler);

            // Send Message.
            var stringMessage = new Message<MessageType, StringMessage>(new StringMessage(message));
            var data = stringMessage.Serialize();
            _simpleServer.NetManager.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);

            while (!isDelivered)
                Thread.Sleep(16);

            Assert.Equal(message, delivered);
        }

        [Fact(Timeout = 1000)]
        public void PassVector3()
        {
            Vector3 message = new Vector3(1.0F, 235.0F, 100.0F);
            Vector3 delivered = null;
            bool isDelivered = false;

            // Message handling method
            void Handler(ref NetMessage<Vector3> netMessage)
            {
                delivered = netMessage.Message;
                isDelivered = true;
            }

            // Setup client
            _simpleClient.MessageHandler.AddOrOverrideHandler<Vector3>(MessageType.Vector3, Handler);

            // Send Message.
            var vectorMessage = new Message<MessageType, Vector3>(message);
            var data = vectorMessage.Serialize();
            _simpleServer.NetManager.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);

            while (!isDelivered)
                Thread.Sleep(16);

            Assert.Equal(message, delivered);
        }
    }
}
