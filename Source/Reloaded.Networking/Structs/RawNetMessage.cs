using LiteNetLib;

namespace Reloaded.Messaging.Structs
{
    public struct RawNetMessage
    {
        public byte[] Message { get; private set; }
        public NetPeer Peer { get; private set; }
        public NetPacketReader PacketReader { get; private set; }
        public DeliveryMethod DeliveryMethod { get; private set; }

        public RawNetMessage(byte[] message, NetPeer peer, NetPacketReader packetReader, DeliveryMethod deliveryMethod)
        {
            Message = message;
            Peer = peer;
            PacketReader = packetReader;
            DeliveryMethod = deliveryMethod;
        }
    }
}
