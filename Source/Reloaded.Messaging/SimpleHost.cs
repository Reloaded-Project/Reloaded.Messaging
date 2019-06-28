using LiteNetLib;
using Reloaded.Messaging.Structs;

namespace Reloaded.Messaging
{
    /// <summary>
    /// Provides a simple client or host based off of LiteNetLib
    /// </summary>
    public class SimpleHost<TMessageType> where TMessageType : unmanaged
    {
        public string Password { get; set; }
        public bool AcceptClients { get; set; }
        public MessageHandler<TMessageType> MessageHandler { get; private set; }

        public EventBasedNetListener Listener { get; private set; }
        public NetManager NetManager { get; private set; }

        public SimpleHost(bool acceptClients, string password = "")
        {
            Password = password;
            AcceptClients = acceptClients;
            MessageHandler = new MessageHandler<TMessageType>();
            Listener = new EventBasedNetListener();
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;

            NetManager = new NetManager(Listener);
            NetManager.UnsyncedEvents = true;
            NetManager.AutoRecycle = true;
        }

        private void ListenerOnConnectionRequestEvent(ConnectionRequest request)
        {
            if (AcceptClients)
                request.AcceptIfKey(Password);
            else
                request.Reject();
        }

        /* On each message received. */
        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
        {
            byte[] rawBytes = reader.GetRemainingBytes();
            var rawNetMessage = new RawNetMessage(rawBytes, peer, reader, deliverymethod);
            MessageHandler.Handle(ref rawNetMessage);
        }
    }
}
